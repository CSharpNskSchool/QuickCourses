using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.DataInterfaces;
using QuickCourses.Api.Extentions;
using QuickCourses.Model.Interaction;
using QuickCourses.Model.Primitives;
using QuickCourses.Model.Progress;

namespace QuickCourses.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly ICourseRepository courseRepository;
        private readonly ICourseProgressRepository courseProgressRepository;
        private readonly IUserRepository userRepository;

        public UsersController(ICourseProgressRepository courseProgressRepository, IUserRepository userRepository, ICourseRepository courseRepository)
        {
            this.courseProgressRepository = courseProgressRepository;
            this.userRepository = userRepository;
            this.courseRepository = courseRepository;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody]User user)
        {
            if (user?.Name == null)
            {
                return BadRequest($"Invalid user info");
            }

            if (await userRepository.Contains(user.Id))
            {
                return BadRequest($"User with id = {user.Id} already exist");
            }


            await userRepository.Insert(user);
            var uri = Request.GetUri();
            return Created($"{uri}/{user.Id}", user);
        }

        [HttpGet("{idUser:int}/courses")]
        public async Task<IActionResult> GetAllCoursesProgresses(int idUser)
        {
            var result = await courseProgressRepository.GetAll(idUser);
            return Ok(result);
        }

        [HttpPost("{idUser:int}/courses")]
        public async Task<IActionResult> StartCourse(int idUser, [FromBody]CourseStartOptions startOptions)
        {
            if (startOptions == null)
            {
                return BadRequest("CourseStartOptions is null");
            }
            
            var course = await courseRepository.Get(startOptions.CourseId);

            if (course == null)
            {
                return BadRequest($"Invalid course id = {startOptions.CourseId}");
            }

            if (!await userRepository.Contains(startOptions.UserId))
            {
                return BadRequest($"Invalid user id = {startOptions.UserId}");
            }

            var courseProgress = course.CreateProgress(startOptions.UserId);
            var uri = Request.GetUri();
            return Created($"{uri}/{courseProgress.CourceId}", courseProgress);
        }

        [HttpGet("{userId:int}/course/{courseId:int}")]
        public async Task<IActionResult> GetCourseProgressById(int userId, int courseId)
        {
            var result = await courseProgressRepository.Get(userId, courseId);
            if (result == null)
            {
                return BadRequest($"Invalid combination of usersId = {userId} and courseId = {courseId}");
            }

            return Ok(result);
        }

        [HttpGet("{userId:int}/course/{courseId:int}/lessons/{lessonId:int}")]
        public async Task<IActionResult> GetLessonProgressById(int userId, int courseId, int lessonId)
        {
            var result = await GetLessonProgress(userId, courseId, lessonId);
            if (result == null)
            {
                return BadRequest($"Invalid combination of usersId = {userId}, courseId = {courseId}, lessonId = {lessonId}");
            }

            return Ok(result);
        }

        [HttpGet("{userId:int}/course/{courseId:int}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> GetAllSteps(int userId, int courseId, int lessonId, int stepId)
        {
            var result = await GetStepProgress(userId, courseId, lessonId, stepId);
            if (result == null)
            {
                return BadRequest(
                    $"Invalid combination of usersId = {userId}, courseId = {courseId}, lessonId = {lessonId}, stepId = {stepId}");
            }

            return Ok(result);
        }

        [HttpPost("{userId:int}/course/{courseId:int}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> PostAnswer(int userId, int courseId, int lessonId, int stepId, [FromBody]Answer answer)
        {
            var courseProgress = await courseProgressRepository.Get(userId, courseId);
            if (courseProgress == null)
            {
                return BadRequest($"Invalid combination of userId = {userId} and courseId = {courseId}");
            }

            var course = await courseRepository.Get(courseId);

            var question = course.GetQuestion(lessonId, stepId, answer.QuestionId);

            if (question == null)
            {
                return BadRequest($"Question {answer.QuestionId} doesn't exist");
            }

            var result = question.GetQuestionState(answer);
            courseProgress.Update(lessonId, courseId, stepId, result);
            await courseProgressRepository.Update(courseProgress);

            return Ok(result);
        }

        private async Task<LessonProgress> GetLessonProgress(int userId, int courseId, int lessonId)
        {
            var course = await courseProgressRepository.Get(userId, courseId);
            if (course == null)
            {
                return null;
            }

            if (course.LessonProgresses.Count < lessonId)
            {
                return null;
            }

            return course.LessonProgresses[lessonId];
        }

        private async Task<LessonStepProgress> GetStepProgress(int userId, int courseId, int lessonId, int stepId)
        {
            var lesson = await GetLessonProgress(userId, courseId, lessonId);
            if (lesson == null)
            {
                return null;
            }

            if (lesson.LessonStepProgress.Count < stepId)
            {
                return null;
            }

            return lesson.LessonStepProgress[stepId];
        }
    }
}
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Extensions;
using QuickCourses.Models.Interaction;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v0/users")]
    public class UsersController : ControllerBase
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
                return BadRequest("Invalid user info");
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

            if (await courseProgressRepository.Contains(startOptions.UserId, startOptions.CourseId))
            {
                return BadRequest($"User id = {startOptions.UserId} has course id = {startOptions.CourseId}");
            }

            var courseProgress = course.CreateProgress(startOptions.UserId);
            await courseProgressRepository.Insert(courseProgress);
            var uri = Request.GetUri();
            return Created($"{uri}/{courseProgress.CourceId}", courseProgress);
        }

        [HttpGet("{userId:int}/courses/{courseId}")]
        public async Task<IActionResult> GetCourseProgressById(int userId, string courseId)
        {
            var result = await courseProgressRepository.Get(userId, courseId);
            if (result == null)
            {
                return NotFound($"Invalid combination of usersId = {userId} and courseId = {courseId}");
            }

            return Ok(result);
        }

        [HttpGet("{userId:int}/courses/{courseId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> GetLessonProgressById(int userId, string courseId, int lessonId)
        {
            var result = await GetLessonProgress(userId, courseId, lessonId);
            if (result == null)
            {
                return NotFound($"Invalid combination of usersId = {userId}, courseId = {courseId}, lessonId = {lessonId}");
            }

            return Ok(result);
        }

        [HttpGet("{userId:int}/courses/{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> GetLessonStep(int userId, string courseId, int lessonId, int stepId)
        {
            var lesson = await GetLessonProgress(userId, courseId, lessonId);
            if (lesson == null)
            {
                return NotFound($"Invalid combination of usersId = {userId}, courseId = {courseId}, lessonId = {lessonId}");
            }

            if (!lesson.LessonStepProgress.TryGetValue(stepId, out var result))
            {
                return NotFound($"Invalid combination of stepId = {stepId}");
            }
            
            return Ok(result);
        }

        [HttpPost("{userId:int}/courses/{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> PostAnswer(int userId, string courseId, int lessonId, int stepId, [FromBody]Answer answer)
        {
            if (answer == null)
            {
                return NotFound($"Invalid combination of userId = {userId} and courseId = {courseId}");
            }
            
            var courseProgress = await courseProgressRepository.Get(userId, courseId);
            if (courseProgress == null)
            {
                return NotFound($"Invalid combination of userId = {userId} and courseId = {courseId}");
            }

            var course = await courseRepository.Get(courseId);

            var question = course.GetQuestion(lessonId, stepId, answer.QuestionId);

            if (question == null)
            {
                return NotFound($"Question {answer.QuestionId} doesn't exist");
            }

            var questionState = courseProgress
                .LessonProgresses[lessonId]
                .LessonStepProgress[stepId]
                .QuestionStates[answer.QuestionId];
            
            var result = question.GetQuestionState(answer, questionState.CurrentAttemptsCount + 1);

            if (question.TotalAttemptsCount < result.CurrentAttemptsCount)
            {
                return InvalidOperation("No available attempts");
            }
            
            courseProgress.Update(lessonId, stepId, answer.QuestionId, result);
            await courseProgressRepository.Update(courseProgress);

            return Ok(result);
        }

        private async Task<LessonProgress> GetLessonProgress(int userId, string courseId, int lessonId)
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
    }
}
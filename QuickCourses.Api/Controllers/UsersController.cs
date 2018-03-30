using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Extentions;
using QuickCourses.Models.Errors;
using QuickCourses.Models.Interaction;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v0/users")]
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
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = "Invalid user info"
                };
                return BadRequest(error);
            }

            if (await userRepository.Contains(user.Id))
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"User with id = {user.Id} already exist"
                };
                return BadRequest(error);
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
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = "CourseStartOptions is null"
                };
                return BadRequest(error);
            }
            
            var course = await courseRepository.Get(startOptions.CourseId);

            if (course == null)
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Invalid course id = {startOptions.CourseId}"
                };
                return BadRequest(error);
            }

            if (!await userRepository.Contains(startOptions.UserId))
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Invalid user id = {startOptions.UserId}"
                };
                return BadRequest(error);
            }

            if (await courseProgressRepository.Contains(startOptions.UserId, startOptions.CourseId))
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"User id = {startOptions.UserId} has course id = {startOptions.CourseId}"
                };
                return BadRequest(error);
            }

            var courseProgress = course.CreateProgress(startOptions.UserId);
            await courseProgressRepository.Insert(courseProgress);
            var uri = Request.GetUri();
            return Created($"{uri}/{courseProgress.CourceId}", courseProgress);
        }

        [HttpGet("{userId:int}/courses/{courseId}")]
        public async Task<IActionResult> GetCourseProgressById(int userId, ObjectId courseId)
        {
            var result = await courseProgressRepository.Get(userId, courseId);
            if (result == null)
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Invalid combination of usersId = {userId} and courseId = {courseId}"
                };
                return BadRequest(error);
            }

            return Ok(result);
        }

        [HttpGet("{userId:int}/courses/{courseId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> GetLessonProgressById(int userId, ObjectId courseId, int lessonId)
        {
            var result = await GetLessonProgress(userId, courseId, lessonId);
            if (result == null)
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Invalid combination of usersId = {userId}, courseId = {courseId}, lessonId = {lessonId}"
                };
                return BadRequest(error);
            }

            return Ok(result);
        }

        [HttpGet("{userId:int}/courses/{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> GetLessonStep(int userId, ObjectId courseId, int lessonId, int stepId)
        {
            var lesson = await GetLessonProgress(userId, courseId, lessonId);
            if (lesson == null)
            {
                var error = new Error {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Invalid combination of usersId = {userId}, courseId = {courseId}, lessonId = {lessonId}"
                };

                return BadRequest(error);
            }

            if (!lesson.LessonStepProgress.TryGetValue(stepId, out var result))
            {
                var error = new Error {
                    Code = Error.ErrorCode.BadArgument,
                    Message =
                        $"Invalid combination of stepId = {stepId}"
                };

                return BadRequest(error);
            }
            
            return Ok(result);
        }

        [HttpPost("{userId:int}/courses/{courseIdString}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> PostAnswer(int userId, string courseIdString, int lessonId, int stepId, [FromBody]Answer answer)
        {
            var courseId = ObjectId.Parse(courseIdString);
            var courseProgress = await courseProgressRepository.Get(userId, courseId);
            if (courseProgress == null)
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Invalid combination of userId = {userId} and courseId = {courseId}"
                };
                return BadRequest(error);
            }

            var course = await courseRepository.Get(courseId);

            var question = course.GetQuestion(lessonId, stepId, answer.QuestionId);

            if (question == null)
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Question {answer.QuestionId} doesn't exist"
                };
                return BadRequest(error);
            }

            var result = question.GetQuestionState(answer);
            courseProgress.Update(lessonId, stepId, answer.QuestionId, result);
            await courseProgressRepository.Update(courseProgress);

            return Ok(result);
        }

        private async Task<LessonProgress> GetLessonProgress(int userId, ObjectId courseId, int lessonId)
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
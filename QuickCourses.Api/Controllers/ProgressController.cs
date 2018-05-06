using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Extensions;
using QuickCourses.Models.Interaction;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Controllers
{
    [Authorize]
    [Route("api/v1/progress")]
    [Produces("application/json")]
    public class ProgressController : ControllerBase
    {
        private readonly IRepository<Course> courseRepository;
        private readonly IProgressRepository progressRepository;

        public ProgressController(
            IProgressRepository progressRepository, 
            IRepository<Course> courseRepository)
        {
            this.progressRepository = progressRepository;
            this.courseRepository = courseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoursesProgresses([FromQuery]string userId)
        {
            if (!User.IsInRole("Client") || string.IsNullOrEmpty(userId))
            {
                userId = User.GetId();
            }
                
            var result = await progressRepository.GetAllByUserAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> StartCourse([FromBody]CourseStartOptions startOptions, [FromQuery]string userId)
        {
            if (startOptions == null)
            {
                return BadRequest("CourseStartOptions is null");
            }

            if (!User.IsInRole("Client") || string.IsNullOrEmpty(userId))
            {
                userId = User.GetId();
            }
            
            if (!IdIsValid(userId))
            {
                return Forbid($"The user has no rights or the id = {userId} is invalid");
            }

            var course = await courseRepository.GetAsync(startOptions.CourseId);

            if (course == null)
            {
                return BadRequest($"Invalid course id = {startOptions.CourseId}");
            }
            
            if (await progressRepository.ContainsAsync(userId, course.Id))
            {
                return BadRequest($"User id = {userId} hasn't course with id = {startOptions.CourseId}");
            }

            var courseProgress = course.CreateProgress(userId);
            courseProgress.Id = await progressRepository.InsertAsync(courseProgress);

            var uri = Request.GetUri();
            return Created($"{uri}/{courseProgress.CourceId}", courseProgress);
        }

        [HttpGet("{progressId}")]
        public async Task<IActionResult> GetCourseProgress(string progressId)
        {
            var result = await progressRepository.GetAsync(progressId);

            if (result == null)
            {
                return NotFound($"Invalid progressId = {progressId}");
            }

            var userId = result.UserId;

            if (!IdIsValid(userId))
            {
                return Forbid($"The user has no rights or the id = {userId} is invalid");
            }
            
            return Ok(result);
        }

        [HttpGet("{progressId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> GetLessonProgressById(string progressId, int lessonId)
        {
            var result = await GetLessonProgress(progressId, lessonId);

            if (result == null)
            {
                return NotFound($"Invalid combination of progressId = {progressId}, lessonId = {lessonId}");
            }

            return Ok(result);
        }

        [HttpGet("{progressId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> GetLessonStep(string progressId, int lessonId, int stepId)
        {
            var courseProgress = await progressRepository.GetAsync(progressId);

            if (courseProgress == null)
            {
                return NotFound($"Invalid progressId = {progressId}");
            }
            
            var userId = courseProgress.UserId;

            if (!IdIsValid(userId))
            {
                return Forbid($"The user with id = {userId} has no rights");
            }

            courseProgress.LessonProgresses.TryGetValue(lessonId, out var lesson);

            if (lesson == null)
            {
                return NotFound($"Invalid combination of progressId = {progressId}, lessonId = {lessonId}");
            }

            if (!lesson.StepProgresses.TryGetValue(stepId, out var result))
            {
                return NotFound($"Invalid combination of stepId = {stepId}");
            }

            return Ok(result);
        }

        [HttpPost("{progressId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> PostAnswer(string progressId, int lessonId, int stepId, [FromBody]Answer answer)
        {
            if (answer == null)
            {
                return BadRequest("answer is null");
            }
            
            var courseProgress = await progressRepository.GetAsync(progressId);

            if (courseProgress == null)
            {
                return NotFound($"Invalid progressId = {progressId}");
            }
            
            var userId = courseProgress.UserId;

            if (!IdIsValid(userId))
            {
                return Forbid($"The user with id = {userId} has no rights");
            }
            
            var course = await courseRepository.GetAsync(courseProgress.CourceId);

            var question = course.GetQuestion(lessonId, stepId, answer.QuestionId);

            if (question == null)
            {
                return NotFound($"Invalid combination of lessonId = {lessonId}, stepId = {stepId}, questionId = {answer.QuestionId} doesn't exist");
            }

            var questionState = courseProgress
                .LessonProgresses[lessonId]
                .StepProgresses[stepId]
                .QuestionStates[answer.QuestionId];

            var result = questionState.Update(question, answer.SelectedAnswers);

            if (question.TotalAttemptsCount < result.CurrentAttemptsCount)
            {
                return InvalidOperation("No available attempts");
            }

            courseProgress.Update(lessonId, stepId, answer.QuestionId, result);
            await progressRepository.ReplaceAsync(courseProgress.Id, courseProgress);

            return Ok(result);
        }

        private async Task<LessonProgress> GetLessonProgress(string progressId, int lessonId)
        {
            var course = await progressRepository.GetAsync(progressId);
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
        
        private bool IdIsValid(string id)
        {
            if (User.IsInRole("Client"))
            {
                return !string.IsNullOrEmpty(id);
            }

            var userId = User.GetId();
            return userId == id;
        }
    }
}

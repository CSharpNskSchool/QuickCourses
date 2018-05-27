using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Models.Extensions;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Data.Models.Progress;
using QuickCourses.Api.Extensions;
using QuickCourses.Api.Models.Interaction;

namespace QuickCourses.Api.Controllers
{
    [Authorize]
    [Route("api/v1/progress")]
    [Produces("application/json")]
    public class ProgressController : ControllerBase
    {
        private readonly IRepository<CourseData> courseRepository;
        private readonly IProgressRepository progressRepository;

        public ProgressController(
            IProgressRepository progressRepository, 
            IRepository<CourseData> courseRepository)
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
                
            var progressDatas = await progressRepository.GetAllByUserAsync(userId);
            var result = progressDatas.Select(x => x.ToApiModel()).ToList();
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
            courseProgress.SetUpLinks();

            var result = courseProgress.ToApiModel();

            var uri = Request.GetUri();
            return Created($"{uri}/{courseProgress.CourceId}", result);
        }

        [HttpGet("{progressId}")]
        public async Task<IActionResult> GetCourseProgress(string progressId)
        {
            var progressData = await progressRepository.GetAsync(progressId);

            if (progressData == null)
            {
                return NotFound($"Invalid progressId = {progressId}");
            }

            var userId = progressData.UserId;

            if (!IdIsValid(userId))
            {
                return Forbid($"The user has no rights or the id = {userId} is invalid");
            }

            var result = progressData.ToApiModel();
            return Ok(result);
        }

        [HttpGet("{progressId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> GetLessonProgressById(string progressId, int lessonId)
        {
            var lessonProgressData = await GetLessonProgress(progressId, lessonId);

            if (lessonProgressData == null)
            {
                return NotFound($"Invalid combination of progressId = {progressId}, lessonId = {lessonId}");
            }

            var result = lessonProgressData.ToApiModel();

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
            
            if (!courseProgress.LessonProgresses.TryGetValue(lessonId, out var lesson))
            {
                return NotFound($"Invalid combination of progressId = {progressId}, lessonId = {lessonId}");
            }

            if (!lesson.StepProgresses.TryGetValue(stepId, out var stepProgressData))
            {
                return NotFound($"Invalid combination of stepId = {stepId}");
            }

            var result = stepProgressData.ToApiModel();

            return Ok(result);
        }

        [HttpPost("{progressId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> PostAnswer(string progressId, int lessonId, int stepId, [FromBody]Answer answer)
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
            
            var course = await courseRepository.GetAsync(courseProgress.CourceId);

            var question = course.GetQuestion(lessonId, stepId, answer.QuestionId.Value);

            if (question == null)
            {
                return NotFound($"Invalid combination of lessonId = {lessonId}, stepId = {stepId}, questionId = {answer.QuestionId} doesn't exist");
            }

            var questionState = courseProgress
                .LessonProgresses[lessonId]
                .StepProgresses[stepId]
                .QuestionStates[answer.QuestionId.Value];

            var questionStateData = questionState.GetUpdated(question, answer.SelectedAnswers);

            if (question.TotalAttemptsCount < questionStateData.CurrentAttemptsCount)
            {
                return InvalidOperation("No available attempts");
            }

            courseProgress.Update(lessonId, stepId, answer.QuestionId.Value, questionStateData);
            await progressRepository.ReplaceAsync(courseProgress.Id, courseProgress);

            var result = questionStateData.ToApiModel();
            return Ok(result);
        }

        private async Task<LessonProgressData> GetLessonProgress(string progressId, int lessonId)
        {
            var course = await progressRepository.GetAsync(progressId);
            if (course == null)
            {
                return null;
            }

            course.LessonProgresses.TryGetValue(lessonId, out var result);
            return result;
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

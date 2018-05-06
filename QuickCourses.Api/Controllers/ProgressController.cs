﻿using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IProgressRepository courseProgressRepository;

        public ProgressController(IProgressRepository courseProgressRepository, IRepository<Course> courseRepository)
        {
            this.courseProgressRepository = courseProgressRepository;
            this.courseRepository = courseRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCoursesProgresses()
        {
            var userId = User.GetId();
            var result = await courseProgressRepository.GetAllByUserAsync(userId);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> StartCourse([FromBody]CourseStartOptions startOptions)
        {
            if (startOptions == null)
            {
                return BadRequest("CourseStartOptions is null");
            }

            var course = await courseRepository.GetAsync(startOptions.CourseId);

            if (course == null)
            {
                return BadRequest($"Invalid course id = {startOptions.CourseId}");
            }

            var userId = User.GetId();
            
            if (await courseProgressRepository.ContainsAsync(userId, course.Id))
            {
                return BadRequest($"User id = {userId} hasn't course with id = {startOptions.CourseId}");
            }

            var courseProgress = course.CreateProgress(userId);
            courseProgress.Id = await courseProgressRepository.InsertAsync(courseProgress);

            var uri = Request.GetUri();
            return Created($"{uri}/{courseProgress.CourceId}", courseProgress);
        }

        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourseProgress(string courseId)
        {
            var userId = User.GetId();
            var result = await courseProgressRepository.GetAsync(userId, courseId);

            if (result == null)
            {
                return NotFound($"Invalid combination of usersId = {userId} and courseId = {courseId}");
            }

            return Ok(result);
        }

        [HttpGet("{courseId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> GetLessonProgressById(string courseId, int lessonId)
        {
            var userId = User.GetId();

            var result = await GetLessonProgress(userId, courseId, lessonId);

            if (result == null)
            {
                return NotFound($"Invalid combination of userId = {userId}, courseId = {courseId}, lessonId = {lessonId}");
            }

            return Ok(result);
        }

        [HttpGet("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> GetLessonStep(string courseId, int lessonId, int stepId)
        {
            var userId = User.GetId();

            var lesson = await GetLessonProgress(userId, courseId, lessonId);

            if (lesson == null)
            {
                return NotFound($"Invalid combination of userId = {userId}, courseId = {courseId}, lessonId = {lessonId}");
            }

            if (!lesson.StepProgresses.TryGetValue(stepId, out var result))
            {
                return NotFound($"Invalid combination of stepId = {stepId}");
            }

            return Ok(result);
        }

        [HttpPost("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> PostAnswer(string courseId, int lessonId, int stepId, [FromBody]Answer answer)
        {
            var userId = User.GetId();

            if (answer == null)
            {
                return BadRequest("answer is null");
            }
            
            var courseProgress = await courseProgressRepository.GetAsync(userId, courseId);

            if (courseProgress == null)
            {
                return NotFound($"Invalid combination of userId = {userId}, courseId = {courseId}");
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

            var result = question.GetQuestionState(answer, questionState.CurrentAttemptsCount + 1);

            if (question.TotalAttemptsCount < result.CurrentAttemptsCount)
            {
                return InvalidOperation("No available attempts");
            }

            courseProgress.Update(lessonId, stepId, answer.QuestionId, result);
            await courseProgressRepository.ReplaceAsync(courseProgress.Id, courseProgress);

            return Ok(result);
        }

        private async Task<LessonProgress> GetLessonProgress(string userId, string courseId, int lessonId)
        {
            var course = await courseProgressRepository.GetAsync(userId, courseId);
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

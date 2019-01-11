using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Models.Extensions;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Exceptions;
using QuickCourses.Api.Extensions;
using QuickCourses.Api.Filters;
using QuickCourses.Api.Models.Extensions;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Controllers
{
    [Authorize(Roles = "Author")]
    [Produces("application/json")]
    [Route("api/editable_courses")]
    public class EditableCoursesController : Controller
    {
        private readonly ICourseRepository courseRepository;
        private readonly IProgressRepository progressRepository;

        public EditableCoursesController(ICourseRepository courseRepository, IProgressRepository progressRepository)
        {
            this.courseRepository = courseRepository;
            this.progressRepository = progressRepository;
        }

        
        [HttpPost]
        public async Task<IActionResult> PostCourse([FromBody]Course course)
        {
            var courseData = course.ToDataModel();

            var authorId = User.GetId();
            courseData.AuthorId = authorId;
            courseData.Id = courseRepository.GenerateNewId(authorId);

            await courseRepository.InsertAsync(courseData);

            course.Id = courseData.Id;

            var uri = Request.GetUri();
            return Created($"{uri}/{course.Id}", course);
        }

        [Authorize(Roles = "Author")]
        [AuthorCheckFilter]
        [HttpDelete("{courseId}")]
        public async Task<IActionResult> DeleteCourse(string courseId)
        {
            var successfullyDeleted = await courseRepository.DeleteAsync(courseId);

            return successfullyDeleted ? NoContent() : NotFound($"Invalid course id = {courseId}");
        }

        [Authorize(Roles = "Author")]
        [AuthorCheckFilter]
        [HttpPatch("{courseId}/description")]
        public async Task<IActionResult> PatchDescription(string courseId, [FromBody]Description description)
        {
            var courseData = await GetCourseDataAsync(courseId);

            courseData.DescriptionData = description.ToDataModel();
            courseData.NextVersion();

            await courseRepository.ReplaceAsync(courseData.Id, courseData);

            return NoContent();
        }

        [Authorize(Roles = "Author")]
        [AuthorCheckFilter]
        [HttpPost("{courseId}/lessons")]
        public async Task<IActionResult> PostLesson(string courseId, [FromBody]Lesson lesson)
        {
            var courseData = await GetCourseDataAsync(courseId);

            var lessonData = lesson.ToDataModel();

            courseData.AddLesson(lessonData, out var lessonId);
            courseData.NextVersion();

            await courseRepository.ReplaceAsync(courseId, courseData);

            var uri = Request.GetUri();
            return Created($"{uri}/{lessonId}", lessonData);
        }

        [Authorize(Roles = "Author")]
        [AuthorCheckFilter]
        [HttpPatch("{courseId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> PatchLesson(string courseId, int lessonId, [FromBody]Lesson lesson)
        {
            var courseData = await courseRepository.GetAsync(courseId);

            if (courseData == null)
            {
                throw new NotFoundException($"Invalid course id = {courseId}");
            }

            courseData.ReplaceLesson(lessonId, lesson.ToDataModel());
            courseData.NextVersion();

            await courseRepository.ReplaceAsync(courseId, courseData);

            return NoContent();
        }

        [Authorize(Roles = "Author")]
        [AuthorCheckFilter]
        [HttpDelete("{courseId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> DeleteLesson(string courseId, int lessonId)
        {
            var courseData = await GetCourseDataAsync(courseId);

            if (!courseData.ContainsLesson(lessonId))
            {
                return NotFound($"Invalid lesson id = {lessonId}");
            }

            courseData.RemoveLesson(lessonId);
            courseData.NextVersion();

            await courseRepository.ReplaceAsync(courseId, courseData);

            return NoContent();
        }

        [Authorize(Roles = "Author")]
        [AuthorCheckFilter]
        [HttpPost("{courseId}/lessons/{lessonId:int}/steps")]
        public async Task<IActionResult> PostStep(string courseId, int lessonId, [FromBody]LessonStep lessonStep)
        {
            var courseData = await GetCourseDataAsync(courseId);
            var lessonData = GetLessonData(courseData, lessonId);

            var lessonStepData = lessonStep.ToDataModel();
            lessonData.AddLessonStepData(lessonStepData, out var stepId);
            courseData.NextVersion();

            await courseRepository.ReplaceAsync(courseId, courseData);

            var uri = Request.GetUri();
            return Created($"{uri}/{stepId}", lessonStepData);
        }

        [Authorize(Roles = "Author")]
        [AuthorCheckFilter]
        [HttpPatch("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> PatchStep(string courseId, int lessonId, int stepId, [FromBody]LessonStep lessonStep)
        {
            var courseData = await GetCourseDataAsync(courseId);
            var lesson = GetLessonData(courseData, lessonId);

            if (!lesson.ContainsStep(stepId))
            {
                throw new NotFoundException($"Invalid step id = {stepId}");
            }

            lesson.ReplaceLessonStepData(stepId, lessonStep.ToDataModel());
            courseData.NextVersion();

            await courseRepository.ReplaceAsync(courseId, courseData);

            return NoContent();
        }

        [Authorize(Roles = "Author")]
        [AuthorCheckFilter]
        [HttpDelete("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> DeleteStep(string courseId, int lessonId, int stepId)
        {
            var courseData = await GetCourseDataAsync(courseId);
            var lesson = GetLessonData(courseData, lessonId);

            lesson.RemoveLessonStepData(stepId);
            courseData.NextVersion();

            await courseRepository.ReplaceAsync(courseId, courseData);

            return NoContent();
        }

        [AuthorCheckFilter]
        [HttpPost("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}/questions}")]
        public async Task<IActionResult> PostQuestion(string courseId, int lessonId, int stepId, [FromBody]Question question)
        {
            var courseData = await GetCourseDataAsync(courseId);
            var stepData = GetLessonStepData(courseData, lessonId, stepId);

            var questionData = question.ToDataModel();

            stepData.AddQuestionData(questionData, out var questionId);
            courseData.NextVersion();

            await courseRepository.ReplaceAsync(courseId, courseData);

            var uri = Request.GetUri();
            return Created($"{uri}/{questionId}", questionData);
        }

        [AuthorCheckFilter]
        [HttpPatch("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}/questions/{questionId}")]
        public async Task<IActionResult> PatchQuestion(
            string courseId,
            int lessonId,
            int stepId,
            int questionId,
            [FromBody]Question question)
        {
            var courseData = await GetCourseDataAsync(courseId);
            var stepData = GetLessonStepData(courseData, lessonId, stepId);

            if (stepData.Questions.Count < questionId)
            {
                throw new NotFoundException($"Invalid quistionId = {questionId}");
            }

            var questionData = question.ToDataModel();
            stepData.Questions[questionId] = questionData;
            courseData.NextVersion();

            await courseRepository.ReplaceAsync(courseId, courseData);

            var uri = Request.GetUri();
            return Created($"{uri}/{questionId}", questionData);
        }

        [Authorize(Roles = "Author")]
        [AuthorCheckFilter]
        [HttpDelete("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}/questions/{questionId}")]
        public async Task<IActionResult> DeleteQuestion(
            string courseId,
            int lessonId,
            int stepId,
            int questionId,
            [FromBody]Question question)
        {
            var courseData = await GetCourseDataAsync(courseId);
            var stepData = GetLessonStepData(courseData, lessonId, stepId);

            if (stepData.Questions.Count < questionId)
            {
                throw new NotFoundException($"Invalid quistionId = {questionId}");
            }

            var questionData = question.ToDataModel();
            stepData.Questions[questionId] = questionData;
            courseData.NextVersion();

            await courseRepository.ReplaceAsync(courseId, courseData);

            var uri = Request.GetUri();
            return Created($"{uri}/{questionId}", questionData);
        }

        private async Task<CourseData> GetCourseDataAsync(string courseId)
        {
            var course = await courseRepository.GetAsync(courseId);

            if (course == null)
            {
                throw new NotFoundException($"Invalid course id = {courseId}");
            }

            return course;
        }

        private LessonData GetLessonData(CourseData courseData, int lessonId)
        {
            if (!courseData.Lessons.TryGetValue(lessonId, out var lesson))
            {
                throw new NotFoundException($"Course doesn't contains lesson with id = {lessonId}");
            }

            return lesson;
        }

        private LessonStepData GetLessonStepData(CourseData courseData, int lessonId, int stepId)
        {
            var lesson = GetLessonData(courseData, lessonId);

            if (!lesson.Steps.TryGetValue(stepId, out var result))
            {
                throw new NotFoundException($"Invalid step id = {stepId}");
            }

            return result;
        }
    }
}
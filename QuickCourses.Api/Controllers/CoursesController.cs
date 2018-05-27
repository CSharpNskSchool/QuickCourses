using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Models.Extensions;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Extensions;
using QuickCourses.Api.Models.Extensions;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Controllers
{
    [Authorize]
    [Route("api/v1/courses")]
    [Produces("application/json")]
    public class CoursesController : ControllerBase
    {
        private readonly IRepository<CourseData> courseRepository;

        public CoursesController(IRepository<CourseData> courseRepository)
        {
            this.courseRepository = courseRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await courseRepository.GetAllAsync();
            var result = courses.Select(course => course.ToApiModel()).ToList();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(string id)
        {
            var course = await courseRepository.GetAsync(id);

            if (course == null)
            {
                return NotFound($"Invalid course id = {id}");
            }

            var result = course.ToApiModel();

            return Ok(result);
        }
        
        [HttpGet("{id}/description")]
        public async Task<IActionResult> GetDescription(string id)
        {
            var course = await courseRepository.GetAsync(id);

            if (course == null)
            {
                return NotFound($"Invalid course id = {id}");
            }

            var result = course.DescriptionData.ToApiModel();
            return Ok(result);
        }

        [HttpGet("{id:int}/lessons")]
        public async Task<IActionResult> GetAllLessons(string id)
        {
            var course = await courseRepository.GetAsync(id);

            if (course == null)
            {
                return NotFound($"Invalid course id = {id}");
            }

            var result = course.Lessons.Select(lesson => lesson.ToApiModel()).ToList();
            return Ok(result);
        }

        [HttpGet("{courseId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> GetLessonById(string courseId, int lessonId)
        {
            var lesson = await GetLesson(courseId, lessonId);

            if(lesson == null)
            {
                return NotFound($"Invalid combination course id = {courseId}, level id = {lessonId}");
            }

            var result = lesson.ToApiModel();
            return Ok(result);
        }

        [HttpGet("{courseId}/lessons/{lessonId:int}/steps")]
        public async Task<IActionResult> GetAllSteps(string courseId, int lessonId)
        {
            var level = await GetLesson(courseId, lessonId);

            if (level == null)
            {
                return NotFound($"Invalid combination course id = {courseId}, level id = {lessonId}");
            }

            var result = level.Steps.Select(step => step.ToApiModel()).ToList();
            return Ok(result);
        }

        [HttpGet("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> GetStepById(string courseId, int lessonId, int stepId)
        {
            var level = await GetLesson(courseId, lessonId);

            if (level == null)
            {
                return NotFound($"Invalid combination course id = {courseId}, level id = {lessonId}");
            }

            if (!level.Steps.TryGetValue(stepId, out var step))
            {
                return NotFound($"Invalid step id = {stepId}");
            }

            var result = step.ToApiModel();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> PostCourse([FromBody]Course course)
        {
            var courseData = course.ToDataModel();
            var id = await courseRepository.InsertAsync(courseData);

            course.Id = id;

            var uri = Request.GetUri();
            return Created($"{uri}/{id}", course);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(string courseId)
        {
            var successfullyDeleted = await courseRepository.DeleteAsync(courseId);

            return successfullyDeleted ? NoContent() : NotFound($"Invalid course id = {courseId}");
        }

        [HttpPatch("{id}/description")]
        public async Task<IActionResult> PatchDescription(string courseId, [FromBody]Description description)
        {
            var course = await courseRepository.GetAsync(courseId);

            if (course == null)
            {
                return NotFound($"Ivalid course id = {courseId}");
            }

            course.DescriptionData = description.ToDataModel();

            await courseRepository.ReplaceAsync(course.Id, course);

            return NoContent();
        }

        [HttpPost("{courseId}/lessons")]
        public async Task<IActionResult> PostLesson(string courseId, [FromBody]Lesson lesson)
        {
            var course = await courseRepository.GetAsync(courseId);

            if (course == null)
            {
                return NotFound($"Invalid course id = {courseId}");
            }

            course.AddLesson(lesson.ToDataModel());
            await courseRepository.ReplaceAsync(courseId, course);

            return Created();
        }

        [HttpPatch("{courseId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> PatchLesson(string courseId, int lessonId, [FromBody]Lesson lesson)
        {
            var course = await courseRepository.GetAsync(courseId);

            if (course == null)
            {
                return NotFound($"Invalid course id = {courseId}");
            }

            course.ReplaceLesson(lessonId, lesson.ToDataModel());

            return NoContent();
        }

        [HttpDelete("{courseId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> DeleteLesson(string courseId, int lessonId)
        {
            var course = await courseRepository.GetAsync(courseId);

            if (course == null)
            {
                return NotFound($"Invalid course id = {courseId}");
            }

            if (!course.ContainsLesson(lessonId))
            {
                return NotFound($"Invalid lesson id = {lessonId}");
            }

            course.RemoveLesson(lessonId);

            return NoContent();
        }

        [HttpPost("{courseId}/lessons/{lessonId:int}/steps")]
        public async Task<IActionResult> PostStep(string courseId, int lessonId, [FromBody]LessonStep lessonStep)
        {
            var lesson = await GetLesson(courseId, lessonId);

            if (lesson == null)
            {
                return NotFound($"Invalid combination course id = {courseId}, level id = {lessonId}");
            }

            lesson.AddLessonStepData(lessonStep.ToDataModel());

            return Created();
        }

        [HttpPatch("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> PatchStep(string courseId, int lessonId, int stepId, [FromBody]LessonStep lessonStep)
        {
            var lesson = await GetLesson(courseId, lessonId);

            if (lesson == null)
            {
                return NotFound($"Invalid combination course id = {courseId}, level id = {lessonId}");
            }

            if (!lesson.ContainsStep(stepId))
            {
                return NotFound($"Invalid step id = {stepId}");
            }

            lesson.ReplaceLessonStepData(stepId, lessonStep.ToDataModel());

            return NoContent();
        }

        [HttpDelete("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> DeleteStep(string courseId, int lessonId, int stepId)
        {
            var lesson = await GetLesson(courseId, lessonId);

            if (lesson == null)
            {
                return NotFound($"Invalid combination course id = {courseId}, level id = {lessonId}");
            }

            if (!lesson.ContainsStep(stepId))
            {
                return NotFound($"Invalid step id = {stepId}");
            }

            lesson.RemoveLessonStepData(stepId);

            return NoContent();
        }

        private async Task<LessonData> GetLesson(string courseId, int lessonId)
        {
            var course = await courseRepository.GetAsync(courseId);

            if (course == null)
            {
                return null;
            }

            course.Lessons.TryGetValue(lessonId, out var result);
            return result;
        }
    }
}

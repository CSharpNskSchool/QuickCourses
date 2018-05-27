using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Models.Extensions;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Exceptions;
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
        private readonly ICourseRepository courseRepository;

        public CoursesController(ICourseRepository courseRepository)
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

        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetCourse(string courseId)
        {
            var course = await GetCourseData(courseId);
            var result = course.ToApiModel();
            return Ok(result);
        }
        
        [HttpGet("{courseId}/description")]
        public async Task<IActionResult> GetDescription(string courseId)
        {
            var course = await GetCourseData(courseId);
            var result = course.DescriptionData.ToApiModel();
            return Ok(result);
        }

        [HttpGet("{courseId}/lessons")]
        public async Task<IActionResult> GetAllLessons(string courseId)
        {
            var course = await GetCourseData(courseId);

            var result = course.Lessons
                .Select(lesson => lesson.ToApiModel())
                .ToList();

            return Ok(result);
        }

        [HttpGet("{courseId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> GetLessonById(string courseId, int lessonId)
        {
            var lesson = await GetLessonData(courseId, lessonId);
            var result = lesson.ToApiModel();
            return Ok(result);
        }

        [HttpGet("{courseId}/lessons/{lessonId:int}/steps")]
        public async Task<IActionResult> GetAllSteps(string courseId, int lessonId)
        {
            var level = await GetLessonData(courseId, lessonId);
            var result = level.Steps.Select(step => step.ToApiModel()).ToList();
            return Ok(result);
        }

        [HttpGet("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> GetStepById(string courseId, int lessonId, int stepId)
        {
            var level = await GetLessonData(courseId, lessonId);
            if (!level.Steps.TryGetValue(stepId, out var step))
            {
                return NotFound($"Invalid step id = {stepId}");
            }

            var result = step.ToApiModel();
            return Ok(result);
        }

        [Authorize(Roles = "Author")]
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(string courseId)
        {
            if (!IsUserAuthor(courseId))
            {
                return UserIsNotAuthor();
            }

            var successfullyDeleted = await courseRepository.DeleteAsync(courseId);

            return successfullyDeleted ? NoContent() : NotFound($"Invalid course id = {courseId}");
        }

        [Authorize(Roles = "Author")]
        [HttpPatch("{id}/description")]
        public async Task<IActionResult> PatchDescription(string courseId, [FromBody]Description description)
        {
            var courseData = await GetCourseData(courseId);

            courseData.DescriptionData = description.ToDataModel();

            await courseRepository.ReplaceAsync(courseData.Id, courseData);

            return NoContent();
        }

        [Authorize(Roles = "Author")]
        [HttpPost("{courseId}/lessons")]
        public async Task<IActionResult> PostLesson(string courseId, [FromBody]Lesson lesson)
        {
            var course = await GetCourseData(courseId);

            course.AddLesson(lesson.ToDataModel());
            await courseRepository.ReplaceAsync(courseId, course);

            return Created();
        }

        [Authorize(Roles = "Author")]
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

        [Authorize(Roles = "Author")]
        [HttpDelete("{courseId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> DeleteLesson(string courseId, int lessonId)
        {
            var course = await GetCourseData(courseId);

            if (!course.ContainsLesson(lessonId))
            {
                return NotFound($"Invalid lesson id = {lessonId}");
            }

            course.RemoveLesson(lessonId);

            return NoContent();
        }

        [Authorize(Roles = "Author")]
        [HttpPost("{courseId}/lessons/{lessonId:int}/steps")]
        public async Task<IActionResult> PostStep(string courseId, int lessonId, [FromBody]LessonStep lessonStep)
        {
            var lesson = await GetLessonData(courseId, lessonId);

            if (lesson == null)
            {
                return NotFound($"Invalid combination course id = {courseId}, level id = {lessonId}");
            }

            lesson.AddLessonStepData(lessonStep.ToDataModel());

            return Created();
        }

        [Authorize(Roles = "Author")]
        [HttpPatch("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> PatchStep(string courseId, int lessonId, int stepId, [FromBody]LessonStep lessonStep)
        {
            var lesson = await GetLessonData(courseId, lessonId);

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

        [Authorize(Roles = "Author")]
        [HttpDelete("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> DeleteStep(string courseId, int lessonId, int stepId)
        {
            var course = await GetCourseData(courseId);



            var lesson = await GetLessonData(courseId, lessonId);

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

        private bool IsUserAuthor(string courseId)
        {
            var userId = User.GetId();
            return courseId.StartsWith(userId);
        }

        private IActionResult UserIsNotAuthor()
        {
            return Forbid($"User with id = {User.GetId()} isn't author of course");
        }

        private async Task<CourseData> GetCourseData(string courseId)
        {
            var course = await courseRepository.GetAsync(courseId);

            if (course == null)
            {
                throw new NotFoundException($"Invalid course id = {courseId}");
            }

            return course;
        }

        private async Task<LessonData> GetLessonData(string courseId, int lessonId)
        {
            var course = await GetCourseData(courseId);

            if (!course.Lessons.TryGetValue(lessonId, out var result))
            {
                throw new NotFoundException($"Invalid lesson id = {lessonId}");
            }

            return result;
        }
    }
}

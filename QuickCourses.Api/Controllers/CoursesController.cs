using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Extentions;
using QuickCourses.Models.Errors;
using QuickCourses.Models.Primitives;

namespace QuickCourses.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v0/courses")]
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
            var result = await courseRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(string id)
        {
            var result = await courseRepository.Get(id);

            if (result == null)
            {
                return NotFound($"Invalid course id = {id}");
            }

            return Ok(result);
        }
        
        [HttpGet("{id}/description")]
        public async Task<IActionResult> GetDescription(string id)
        {
            var course = await courseRepository.Get(id);

            if (course == null)
            {
                return NotFound($"Invalid course id = {id}");
            }

            var result = course.Description;

            return Ok(result);
        }

        [HttpGet("{id:int}/lessons")]
        public async Task<IActionResult> GetAllLessons(string id)
        {
            var course = await courseRepository.Get(id);

            if (course == null)
            {
                return NotFound($"Invalid course id = {id}");
            }

            return Ok(course.Lessons);
        }

        [HttpGet("{courseId}/lessons/{lessonId:int}")]
        public async Task<IActionResult> GetLessonById(string courseId, int lessonId)
        {
            var result = await GetLesson(courseId, lessonId);

            if(result == null)
            {
                return NotFound($"Invalid combination course id = {courseId}, level id = {lessonId}");
            }

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

            return Ok(level.Steps);
        }

        [HttpGet("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> GetStepById(string courseId, int lessonId, int stepId)
        {
            var level = await GetLesson(courseId, lessonId);

            if (level == null)
            {
                return NotFound($"Invalid combination course id = {courseId}, level id = {lessonId}");
            }

            if (!level.Steps.TryGetValue(stepId, out var result))
            {
                return NotFound($"Invalid step id = {stepId}");
            }

            return Ok(result);
        }

        private async Task<Lesson> GetLesson(string courseId, int lessonId)
        {
            var course = await courseRepository.Get(courseId);

            if (course == null)
            {
                return null;
            }

            course.Lessons.TryGetValue(lessonId, out var result);
            return result;
        }
    }
}

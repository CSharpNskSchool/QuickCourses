using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.DataInterfaces;
using QuickCourses.Model.Primitives;

namespace QuickCourses.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/courses")]
    public class CoursesController : Controller
    {
        private readonly ICourseRepository courseRepository;

        public CoursesController(ICourseRepository courseRepository)
        {
            this.courseRepository = courseRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllCourses()
        {
            var result = await courseRepository.GetAllCourses();
            return Ok(result);
        }

        [HttpGet("/{id:int}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            var result = await courseRepository.GetCourse(id);
            if (result == null)
            {
                return BadRequest($"Invalid course id = {id}");
            }

            return Ok(result);
        }

        [HttpGet("/{id:int}/lessons")]
        public async Task<IActionResult> GetAllLessons(int id)
        {
            var course = await courseRepository.GetCourse(id);
            if (course == null)
            {
                return BadRequest($"Invalid course id = {id}");
            }

            return Ok(course.Lessons);
        }

        [HttpGet("/{courseId:int}/lessons/{lessonId:int}")]
        public async Task<IActionResult> GetLessonById(int courseId, int lessonId)
        {
            var result = await GetLesson(courseId, lessonId);

            if(result == null)
            { 
                return BadRequest($"Invalid combination course id = {courseId}, level id = {lessonId}");
            }

            return Ok(result);
        }

        [HttpGet("/{courseId:int}/lessons/{lessonId:int}/steps")]
        public async Task<IActionResult> GetAllSteps(int courseId, int lessonId)
        {
            var level = await GetLesson(courseId, lessonId);
            if (level == null)
            {
                return BadRequest($"Invalid combination course id = {courseId}, level id = {lessonId}");
            }

            return Ok(level.Steps);
        }

        [HttpGet("/{courseId:int}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> GetStepById(int courseId, int lessonId, int stepId)
        {
            var level = await GetLesson(courseId, lessonId);
            if (level == null)
            {
                return BadRequest($"Invalid combination course id = {courseId}, level id = {lessonId}");
            }

            if (level.Steps.Count < stepId)
            {
                return BadRequest($"Invalid step id = {stepId}");
            }

            return Ok(level.Steps[stepId]);
        }

        private async Task<Lesson> GetLesson(int courseId, int lessonId)
        {
            var course = await courseRepository.GetCourse(courseId);
            if (course == null)
            {
                return null;
            }

            if (course.Lessons.Count < lessonId)
            {
                return null;
            }

            return course.Lessons[lessonId];
        }
    }
}

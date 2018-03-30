using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Extentions;
using QuickCourses.Models;
using QuickCourses.Models.Errors;
using QuickCourses.Models.Primitives;

namespace QuickCourses.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v0/courses")]
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
            var result = await courseRepository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCourse(ObjectId id)
        {
            var result = await courseRepository.Get(id);

            if (result == null)
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Invalid course id = {id}"
                };
                return BadRequest(error);
            }

            return Ok(result);
        }

        [HttpGet("{id:int}/lessons")]
        public async Task<IActionResult> GetAllLessons(ObjectId id)
        {
            var course = await courseRepository.Get(id);

            if (course == null)
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Invalid course id = {id}"
                };
                return BadRequest(error);
            }

            return Ok(course.Lessons);
        }

        [HttpGet("{courseId:int}/lessons/{lessonId:int}")]
        public async Task<IActionResult> GetLessonById(ObjectId courseId, int lessonId)
        {
            var result = await GetLesson(courseId, lessonId);

            if(result == null)
            { 
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Invalid combination course id = {courseId}, level id = {lessonId}"
                };
                return BadRequest(error);
            }

            return Ok(result);
        }

        [HttpGet("{courseId:int}/lessons/{lessonId:int}/steps")]
        public async Task<IActionResult> GetAllSteps(ObjectId courseId, int lessonId)
        {
            var level = await GetLesson(courseId, lessonId);

            if (level == null)
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Invalid combination course id = {courseId}, level id = {lessonId}"
                };
                return BadRequest(error);
            }

            return Ok(level.Steps);
        }

        [HttpGet("{courseId:int}/lessons/{lessonId:int}/steps/{stepId:int}")]
        public async Task<IActionResult> GetStepById(ObjectId courseId, int lessonId, int stepId)
        {
            var level = await GetLesson(courseId, lessonId);

            if (level == null)
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Invalid combination course id = {courseId}, level id = {lessonId}"
                };
                return BadRequest(error);
            }

            if (!level.Steps.TryGetValue(stepId, out var result))
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.BadArgument,
                    Message = $"Invalid step id = {stepId}"
                };
                return BadRequest(error);
            }

            return Ok(result);
        }

        private async Task<Lesson> GetLesson(ObjectId courseId, int lessonId)
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

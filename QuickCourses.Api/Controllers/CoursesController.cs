using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Models.Extensions;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Extensions;
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
        [Produces(typeof(IList<Course>))]
        public async Task<IActionResult> GetAllCourses()
        {
            var courses = await courseRepository.GetAllAsync();
            var result = courses.Select(course => course.ToApiModel()).ToList();
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Produces(typeof(Course))]
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
        [Produces(typeof(Description))]
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
        [Produces(typeof(IList<Lesson>))]
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
        [Produces(typeof(Lesson))]
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
        [Produces(typeof(IList<LessonStep>))]
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="lessonId"></param>
        /// <param name="stepId"></param>
        /// <returns>LessonStep</returns>
        [HttpGet("{courseId}/lessons/{lessonId:int}/steps/{stepId:int}")]
        [Produces(typeof(LessonStep))]
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
        feature/add-swagger
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

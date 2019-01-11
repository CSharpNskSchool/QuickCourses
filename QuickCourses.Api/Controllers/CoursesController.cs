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
using QuickCourses.Api.Filters;
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
            var course = await GetCourseDataAsync(courseId);
            var result = course.ToApiModel();
            return Ok(result);
        }
        
        [HttpGet("{courseId}/description")]
        public async Task<IActionResult> GetDescription(string courseId)
        {
            var course = await GetCourseDataAsync(courseId);
            var result = course.DescriptionData.ToApiModel();
            return Ok(result);
        }

        [HttpGet("{courseId}/lessons")]
        public async Task<IActionResult> GetAllLessons(string courseId)
        {
            var course = await GetCourseDataAsync(courseId);

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
            var step = await GetLessonStepDataAsync(courseId, lessonId, stepId);
            var result = step.ToApiModel();
            return Ok(result);
        }



        private async Task<LessonData> GetLessonData(string courseId, int lessonId)
        {
            var courseData = await GetCourseDataAsync(courseId);
            var result = GetLessonData(courseData, lessonId);
            return result;
        }

        private async Task<LessonStepData> GetLessonStepDataAsync(string courseId, int lessonId, int stepId)
        {
            var courseData = await GetCourseDataAsync(courseId);
            var result = GetLessonStepData(courseData, lessonId, stepId);
            return result;
        }
    }
}

using System;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.DataInterfaces;

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
        public IActionResult GetAllCourses()
        {
            throw new NotImplementedException();
        }

        [HttpGet("/{id:int}")]
        public IActionResult GetCourse(int id)
        {
            throw new NotImplementedException();;
        }

        [HttpGet("/lessons")]
        public IActionResult GetAllLessons(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("/lessons/{id:int}")]
        public IActionResult GetLesson(int id)
        {
            throw new NotImplementedException();
        }

        [HttpGet("/steps")]
        public IActionResult GetAllSteps()
        {
            throw new NotImplementedException();
        }

        [HttpGet("/steps/{id:int}")]
        public IActionResult GetStep(int id)
        {
            throw new NotImplementedException();
        }
    }
}

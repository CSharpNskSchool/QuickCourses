﻿using System;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.DataInterfaces;
using QuickCourses.Api.Repositories;
using QuickCourses.Model.Interaction;
using QuickCourses.Model.Primitives;

namespace QuickCourses.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/users")]
    public class UsersController : Controller
    {
        private readonly IRunnedCourseRepository runnedCourseRepository;
        private readonly IUserRepository userRepository;

        public UsersController(IRunnedCourseRepository runnedCourseRepository, IUserRepository userRepository)
        {
            this.runnedCourseRepository = runnedCourseRepository;
            this.userRepository = userRepository;
        }

        [HttpPost]
        [Route("")]
        public IActionResult PostUser([FromBody]User user)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{idUser:int}/courses")]
        public IActionResult GetAllCoursesProgresses(int idUser)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("{idUser:int}/courses")]
        public IActionResult GetCourse(int idUser, [FromBody]CourseStartOptions startOptions)
        {
            throw new NotImplementedException(); ;
        }

        [HttpGet]
        [Route("{idUser:int}/course/{idCourse:int}")]
        public IActionResult GetCourseProgress(int idUser)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{idUser:int}/course/{idCourse:int}/lessons/{idLesson:int}")]
        public IActionResult GetLessonProgress(int idUser, int idCourse, int idLesson)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{idUser:int}/course/{idCourse:int}/lessons/{idLesson:int}/steps/{idStep:int}")]
        public IActionResult GetAllSteps(int idUser, int idCourse, int idLesson, int idStep)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("{idUser:int}/course/{idCourse:int}/lessons/{idLesson:int}/steps/{idStep:int}")]
        public IActionResult PostAnswer(int idUser, int idCourse, int idLesson, int idStep, [FromBody]Answer answer)
        {
            throw new NotImplementedException();
        }
    }
}
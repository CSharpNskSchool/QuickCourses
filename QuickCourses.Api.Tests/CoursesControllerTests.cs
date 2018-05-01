using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using QuickCourses.Api.Controllers;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Models.Errors;
using QuickCourses.Models.Primitives;

namespace QuickCourses.Api.Tests
{
    [TestFixture]
    public class CoursesControllerTests
    {
        private CoursesController controller;
        private Course course;

        [SetUp]
        public void Init()
        {
            course = Utilits.CreateCourse();
            controller = CreateCourseController();
        }

        [Test]
        public void GetAllCoursesTest()
        {
            var response = controller.GetAllCourses().Result;

            Utilits.CheckResponseValue<OkObjectResult, List<Course>>(response, new List<Course> {course});
        }

        [Test]
        public void GetCourseTest()
        {
            var response = controller.GetCourse(course.Id).Result;
            
            Utilits.CheckResponseValue<OkObjectResult, Course>(response, course);
        }
        
        [Test]
        public void GetDescriptionTest()
        {
            var response = controller.GetDescription(course.Id).Result;
            
            Utilits.CheckResponseValue<OkObjectResult, Description>(response, course.Description);
        }

        [Test]
        public void GetCourseWithInvalidIdTest()
        {
            var invalidId = ObjectId.GenerateNewId().ToString();
            var response = controller.GetCourse(invalidId).Result;

            var expectedResult = new Error {Code = Error.ErrorCode.NotFound, Message = $"Invalid course id = {invalidId}"};

            Utilits.CheckResponseValue<NotFoundObjectResult, Error>(response, expectedResult);
        }

        [Test]
        public void GetAllLessonsTest()
        {
            var response = controller.GetAllLessons(course.Id).Result;

            var expectedResult = course.Lessons;

            Utilits.CheckResponseValue<OkObjectResult, List<Lesson>>(response, expectedResult);
        }

        [Test]
        public void GetLessonByIdTest()
        {
            var response = controller.GetLessonById(course.Id, 0).Result;

            var expecteResult = course.Lessons[0];

            Utilits.CheckResponseValue<OkObjectResult, Lesson>(response, expecteResult);
        }

        [Test]
        public void GetAllStepsTest()
        {
            var response = controller.GetAllSteps(course.Id, 0).Result;

            var expectedResult = course.Lessons[0].Steps;

            Utilits.CheckResponseValue<OkObjectResult, List<LessonStep>>(response, expectedResult);
        }

        [Test]
        public void GetStepByIdTest()
        {
            var response = controller.GetStepById(course.Id, 0, 0).Result;

            var expectedResult = course.Lessons[0].Steps[0];
            
            Utilits.CheckResponseValue<OkObjectResult, LessonStep>(response, expectedResult);
        }

        private CoursesController CreateCourseController()
        {
            var mockRepo = new Mock<IRepository<Course>>();
            mockRepo
                .Setup(repo => repo.GetAllAsync())
                .Returns(Task.FromResult(new List<Course> {course}));

            mockRepo
                .Setup(repo => repo.GetAsync(course.Id))
                .Returns(Task.FromResult(course));

            var result = new CoursesController(mockRepo.Object);
            return result;
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using QuickCourses.Api.Controllers;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Models.Errors;

namespace QuickCourses.Api.Tests
{
    [TestFixture]
    public class CoursesControllerTests
    {
        private CoursesController controller;
        private CourseData courseData;

        [SetUp]
        public void Init()
        {
            courseData = Utilits.CreateCourse();
            controller = CreateCourseController();
        }

        [Test]
        public void GetAllCoursesTest()
        {
            var response = controller.GetAllCourses().Result;

            Utilits.CheckResponseValue<OkObjectResult, List<CourseData>>(response, new List<CourseData> {courseData});
        }

        [Test]
        public void GetCourseTest()
        {
            var response = controller.GetCourse(courseData.Id).Result;
            
            Utilits.CheckResponseValue<OkObjectResult, CourseData>(response, courseData);
        }
        
        [Test]
        public void GetDescriptionTest()
        {
            var response = controller.GetDescription(courseData.Id).Result;
            
            Utilits.CheckResponseValue<OkObjectResult, DescriptionData>(response, courseData.DescriptionData);
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
            var response = controller.GetAllLessons(courseData.Id).Result;

            var expectedResult = courseData.Lessons;

            Utilits.CheckResponseValue<OkObjectResult, List<LessonData>>(response, expectedResult);
        }

        [Test]
        public void GetLessonByIdTest()
        {
            var response = controller.GetLessonById(courseData.Id, 0).Result;

            var expecteResult = courseData.Lessons[0];

            Utilits.CheckResponseValue<OkObjectResult, LessonData>(response, expecteResult);
        }

        [Test]
        public void GetAllStepsTest()
        {
            var response = controller.GetAllSteps(courseData.Id, 0).Result;

            var expectedResult = courseData.Lessons[0].Steps;

            Utilits.CheckResponseValue<OkObjectResult, List<LessonStepData>>(response, expectedResult);
        }

        [Test]
        public void GetStepByIdTest()
        {
            var response = controller.GetStepById(courseData.Id, 0, 0).Result;

            var expectedResult = courseData.Lessons[0].Steps[0];
            
            Utilits.CheckResponseValue<OkObjectResult, LessonStepData>(response, expectedResult);
        }

        private CoursesController CreateCourseController()
        {
            var mockRepo = new Mock<IRepository<CourseData>>();
            mockRepo
                .Setup(repo => repo.GetAllAsync())
                .Returns(Task.FromResult(new List<CourseData> {courseData}));

            mockRepo
                .Setup(repo => repo.GetAsync(courseData.Id))
                .Returns(Task.FromResult(courseData));

            var result = new CoursesController(mockRepo.Object);
            return result;
        }
    }
}

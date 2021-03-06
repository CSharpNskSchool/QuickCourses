﻿using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using QuickCourses.Api.Controllers;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Models.Extensions;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Data.Models.Progress;
using QuickCourses.Api.Models.Errors;
using QuickCourses.Api.Models.Interaction;

namespace QuickCourses.Api.Tests
{
    [TestFixture]
    public class ProgeressControllerTests
    {
        private CourseData courseData;
        private CourseProgressData courseProgressData;
        private string userId;
        private ClaimsPrincipal user;
        private ProgressController controller;

        [SetUp]
        public void Init()
        {
            courseData = Utilits.CreateCourse();
			courseData.SetUpLinks();
            userId = "1";
            user = new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(ClaimTypes.NameIdentifier, userId)}));
            courseProgressData = courseData.CreateProgress(userId);
            courseProgressData.Id = $"{userId}{courseData.Id}";
			courseProgressData.SetUpLinks();
            controller = CreateProgressController();
        }

        [Test]
        public void StartCourseTest_ValidTest()
        {
            var response = controller.StartCourse(new CourseStartOptions {CourseId = courseData.Id}, null).Result;

            Utilits.CheckResponseValue<CreatedResult, CourseProgressData>(response, courseProgressData);
        }

        [Test]
        public void GetAllCoursesProgressesTest_ValidTest()
        {
            var response = controller.GetAllCoursesProgresses(null).Result;

            Utilits.CheckResponseValue<OkObjectResult, List<CourseProgressData>>(
                response,
                new List<CourseProgressData> {courseProgressData});
        }

        [Test]
        public void GetCourseTest_ValidTest()
        {
            var response = controller.GetCourseProgress(courseProgressData.Id).Result;

            Utilits.CheckResponseValue<OkObjectResult, CourseProgressData>(response, courseProgressData);
        }

        [Test]
        public void GetCourseLesson_ValidTest()
        {
            var response = controller.GetLessonProgressById(courseProgressData.Id, lessonId: 0).Result;

            var expectedResult = courseProgressData.LessonProgresses[0];

            Utilits.CheckResponseValue<OkObjectResult, LessonProgressData>(response, expectedResult);
        }

        [Test]
        public void GetCourseLessonStep_ValidTest()
        {
            var response = controller.GetLessonStep(courseProgressData.Id, lessonId: 0, stepId: 0).Result;

            var expectedResult = courseProgressData.LessonProgresses[0].StepProgresses[0];

            Utilits.CheckResponseValue<OkObjectResult, StepProgressData>(response, expectedResult);
        }
        
        [Test]
        public void PostAnswerTest()
        {
            const int lessonId = 0;
            const int stepId = 0;
            const int questionId = 0;

            var answer = new Answer { QuestionId = questionId, SelectedAnswers = new List<int> { 0 } };
            var result = controller.PostAnswer(courseProgressData.Id, lessonId, stepId, answer).Result;

            var question = courseData.Lessons[lessonId].Steps[stepId].Questions[questionId];
            var expectedValue = question.GetQuestionState().Update(question, answer.SelectedAnswers);
            expectedValue.ProgressId = courseProgressData.Id;
            Utilits.CheckResponseValue<OkObjectResult, QuestionStateData>(result, expectedValue);
        }

        [Test]
        public void GetCourseProgress_InvalidUrl()
        {
            var invalidId = ObjectId.GenerateNewId().ToString();
            var response = controller.GetCourseProgress(invalidId).Result;

            var expectedValue = new Error 
            {
                Code = Error.ErrorCode.NotFound,
                Message = $"Invalid progressId = {invalidId}"
            };

            Utilits.CheckResponseValue<NotFoundObjectResult, Error>(response, expectedValue);
        }

        private ProgressController CreateProgressController()
        {
            var mockCourseProgressRepo = new Mock<IProgressRepository>();
            mockCourseProgressRepo
                .Setup(courseProgressRepo => courseProgressRepo.ContainsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            mockCourseProgressRepo
                .Setup(courseProgressRepo => courseProgressRepo.ContainsAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            mockCourseProgressRepo
                .Setup(courseProgressRepo => courseProgressRepo.InsertAsync(It.IsAny<CourseProgressData>()))
                .Returns(Task.FromResult($"{userId}{courseData.Id}"));

            mockCourseProgressRepo.Setup(repo => repo.GetAsync(userId, courseData.Id))
                .Returns(Task.FromResult(courseProgressData));

            mockCourseProgressRepo.Setup(repo => repo.GetAsync(courseProgressData.Id))
                .Returns(Task.FromResult(courseProgressData));

            mockCourseProgressRepo.Setup(repo => repo.GetAllByUserAsync(userId))
                .Returns(Task.FromResult(new List<CourseProgressData> {courseProgressData}));

            var mockCourseRepo = new Mock<IRepository<CourseData>>();
            mockCourseRepo
                .Setup(courseRepo => courseRepo.GetAsync(courseData.Id))
                .Returns(Task.FromResult(courseData));

            
            var result = new ProgressController(
                mockCourseProgressRepo.Object,
                mockCourseRepo.Object) {
                ControllerContext = { HttpContext = Utilits.CreateContext("http", "host", "path", user) }
            };

            return result;
        }
    }
}

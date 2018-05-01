using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using QuickCourses.Api.Controllers;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Extensions;
using QuickCourses.Models.Errors;
using QuickCourses.Models.Interaction;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Tests
{
    [TestFixture]
    public class ProgeressControllerTests
    {
        private Course course;
        private CourseProgress courseProgress;
        private string userId;
        private ClaimsPrincipal user;
        private ProgressController controller;

        [SetUp]
        public void Init()
        {
            course = Utilits.CreateCourse();
            userId = "1";
            user = new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(ClaimTypes.NameIdentifier, userId)}));
            courseProgress = course.CreateProgress(userId);
            courseProgress.Id = $"{userId}{course.Id}";
            controller = CreateProgressController();
        }

        [Test]
        public void StartCourseTest_ValidTest()
        {
            var response = controller.StartCourse(new CourseStartOptions {CourseId = course.Id}).Result;

            Utilits.CheckResponseValue<CreatedResult, CourseProgress>(response, courseProgress);
        }

        [Test]
        public void GetAllCoursesProgressesTest_ValidTest()
        {
            var response = controller.GetAllCoursesProgresses().Result;

            Utilits.CheckResponseValue<OkObjectResult, List<CourseProgress>>(
                response,
                new List<CourseProgress> {courseProgress});
        }

        [Test]
        public void GetCourseTest_ValidTest()
        {
            var response = controller.GetCourseProgressById(courseProgress.Id).Result;

            Utilits.CheckResponseValue<OkObjectResult, CourseProgress>(response, courseProgress);
        }

        [Test]
        public void GetCourseLesson_ValidTest()
        {
            var response = controller.GetLessonProgressById(course.Id, lessonId: 0).Result;

            var expectedResult = courseProgress.LessonProgresses[0];

            Utilits.CheckResponseValue<OkObjectResult, LessonProgress>(response, expectedResult);
        }

        [Test]
        public void GetCourseLessonStep_ValidTest()
        {
            var response = controller.GetLessonStep(course.Id, lessonId: 0, stepId: 0).Result;

            var expectedResult = courseProgress.LessonProgresses[0].StepProgresses[0];

            Utilits.CheckResponseValue<OkObjectResult, LessonStepProgress>(response, expectedResult);
        }
        
        [Test]
        public void PostAnswerTest()
        {
            const int lessonId = 0;
            const int stepId = 0;
            const int questionId = 0;

            var answer = new Answer { QuestionId = questionId, SelectedAnswers = new List<int> { 0 } };
            var result = controller.PostAnswer(course.Id, lessonId, stepId, answer).Result;

            var question = course.Lessons[lessonId].Steps[stepId].Questions[questionId];
            var expectedValue = question.GetQuestionState(answer, currentAttemptsCount: 1);

            Utilits.CheckResponseValue<OkObjectResult, QuestionState>(result, expectedValue);
        }

        [Test]
        public void GetCourseProgress_InvalidUrl()
        {
            var invalidId = ObjectId.GenerateNewId().ToString();
            var response = controller.GetCourseProgressById(invalidId).Result;

            var expectedValue = new Error 
            {
                Code = Error.ErrorCode.NotFound,
                Message = $"Invalid combination of usersId = {userId} and courseId = {invalidId}"
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
                .Setup(courseProgressRepo => courseProgressRepo.InsertAsync(It.IsAny<CourseProgress>()))
                .Returns(Task.FromResult($"{userId}{course.Id}"));

            mockCourseProgressRepo.Setup(repo => repo.GetAsync(userId, course.Id))
                .Returns(Task.FromResult(courseProgress));

            mockCourseProgressRepo.Setup(repo => repo.GetAsync(courseProgress.Id))
                .Returns(Task.FromResult(courseProgress));

            mockCourseProgressRepo.Setup(repo => repo.GetAllByUserAsync(userId))
                .Returns(Task.FromResult(new List<CourseProgress> {courseProgress}));

            var mockCourseRepo = new Mock<IRepository<Course>>();

            mockCourseRepo
                .Setup(courseRepo => courseRepo.GetAsync(course.Id))
                .Returns(Task.FromResult(course));

            var result = new ProgressController(
                mockCourseProgressRepo.Object,
                mockCourseRepo.Object) {
                ControllerContext = { HttpContext = Utilits.CreateContext("http", "host", "path", user) }
            };

            return result;
        }
    }
}

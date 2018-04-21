using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using QuickCourses.Api.Controllers;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Extentions;
using QuickCourses.Models.Interaction;
using QuickCourses.Models.Primitives;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using QuickCourses.Models.Errors;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Tests
{
    [TestFixture]
    public class UsersControllerTests
    {
        private Course course;
        private CourseProgress courseProgress;
        private User user;
        
        [SetUp]
        public void Init()
        {
            course = new Course
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Description = new Description {Name = "Test Course", Overview = "Course to test Api"},
                Lessons = new List<Lesson>
                {
                    new Lesson
                    {
                        Id = 0,
                        Description =
                            new Description {Name = "Only lesson", Overview = "Only lesson of test course"},
                        Steps = new List<LessonStep>
                        {
                            new LessonStep
                            {
                                Id = 0,
                                EducationalMaterial = new EducationalMaterial
                                {
                                    Description = new Description
                                    {
                                        Name = "Only step",
                                        Overview = "Only step of only lesson of only course"
                                    },
                                    Article = "You must love this API"
                                },
                                Questions = new List<Question>
                                {
                                    new Question
                                    {
                                        Text = "Do you love this API?",
                                        AnswerVariants = new List<AnswerVariant>
                                        {
                                            new AnswerVariant {Id = 0, Text = "Yes"}
                                        },
                                        CorrectAnswers = new List<int> {0},
                                        TotalAttemptsCount = 2
                                    }
                                }
                            }
                        }
                    }
                }
            };

            user = new User {Id = 0, Name = "Test"};

            courseProgress = course.CreateProgress(user.Id);
        }

        [Test]
        public void PostUser_ValidTest()
        {
            var controller = CreatePostUserTestUsersController();

            var response = controller.PostUser(user).Result;

            Utilits.CheckResponseValue<CreatedResult, User>(response, user);
        }

        [Test]
        public void StartCourseTest_ValidTest()
        {
            var controller = CreateStartCourseTestUserController();

            var response = controller.StartCourse(
                user.Id, 
                new CourseStartOptions {CourseId = course.Id, UserId = user.Id}).Result;

            Utilits.CheckResponseValue<CreatedResult, CourseProgress>(response, courseProgress);
        }

        [Test]
        public void GetAllCoursesProgressesTest_ValidTest()
        {
            var mockCoureseProgressRepo = new Mock<ICourseProgressRepository>();
            mockCoureseProgressRepo.Setup(repo => repo.GetAll(user.Id))
                .Returns(Task.FromResult((IEnumerable<CourseProgress>) new[] {courseProgress}));

            var controller = new UsersController(mockCoureseProgressRepo.Object, null, null)
            {
                ControllerContext = {HttpContext = Utilits.CreateContext("http", "host", "path")}
            };

            var response = controller.GetAllCoursesProgresses(user.Id).Result;

            Utilits.CheckResponseValue<OkObjectResult, IEnumerable<CourseProgress>>(response, new[] {courseProgress});
        }

        [Test]
        public void GetCourseTest_ValidTest()
        {
            var controller = CreateGetTestUserController();

            var response = controller.GetCourseProgressById(user.Id, course.Id).Result;

            Utilits.CheckResponseValue<OkObjectResult, CourseProgress>(response, courseProgress);
        }

        [Test]
        public void GetCourseLesson_ValidTest()
        {
            var controller = CreateGetTestUserController();

            var response = controller.GetLessonProgressById(user.Id, course.Id, 0).Result;

            var expectedResult = courseProgress.LessonProgresses[0];

            Utilits.CheckResponseValue<OkObjectResult, LessonProgress>(response, expectedResult);
        }

        [Test]
        public void GetCourseLessonStep_ValidTest()
        {
            var controller = CreateGetTestUserController();

            var response = controller.GetLessonStep(user.Id, course.Id, 0, 0).Result;

            var expectedResult = courseProgress.LessonProgresses[0].LessonStepProgress[0];

            Utilits.CheckResponseValue<OkObjectResult, LessonStepProgress>(response, expectedResult);
        }

        [Test]
        public void PostUser_InvalidUserInfoTest()
        {
            var controller = CreatePostUserTestUsersController();

            var response = controller.PostUser(null).Result;

            var expectedResult = new Error {Code = Error.ErrorCode.BadArgument, Message = "Invalid user info"};

            Utilits.CheckResponseValue<BadRequestObjectResult, Error>(
                response,
                expectedResult);
        }

        [Test]
        public void PostAnswerTest()
        {
            var controller = CreatePostAnswerTestUsersController();

            const int lessonId = 0;
            const int stepId = 0;
            const int questionId = 0;
            
            var answer = new Answer {QuestionId = questionId, SelectedAnswers = new List<int> {0}};
            var response = controller.PostAnswer(user.Id, course.Id, lessonId, stepId, answer).Result;
            var question = course.Lessons[lessonId].Steps[stepId].Questions[questionId];

            var questionState = new QuestionState
            {
                CorrectlySelectedAnswers = new List<int>(),
                SelectedAnswers = new List<int>(),
                LessonId = lessonId,
                StepId = stepId,
                QuestionId = questionId
            };
            
            var expectedValue = question.GetQuestionState(answer, questionState);

            Utilits.CheckResponseValue<OkObjectResult, QuestionState>(response, expectedValue);
        }

        [Test]
        public void GetCourseProgress_InvalidUrl()
        {
            var controller = CreateGetTestUserController();
            var invalidId = ObjectId.GenerateNewId().ToString();

            var response = controller.GetCourseProgressById(0, invalidId).Result;

            Utilits.CheckResponseValue<NotFoundObjectResult, Error>(
                response,
                new Error{Code = Error.ErrorCode.NotFound, Message = $"Invalid combination of usersId = 0 and courseId = {invalidId}"});
        }

        private UsersController CreateStartCourseTestUserController()
        {
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo
                .Setup(userRepo => userRepo.Contains(It.IsAny<int>()))
                .Returns(Task.FromResult(true));

            var mockCoursePrgressRepo = new Mock<ICourseProgressRepository>();
            mockCoursePrgressRepo
                .Setup(courseProgressRepo => courseProgressRepo.Contains(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            mockCoursePrgressRepo
                .Setup(courseProgressRepo => courseProgressRepo.Insert(It.IsAny<CourseProgress>()))
                .Returns(Task.CompletedTask);

            mockCoursePrgressRepo
                .Setup(courseProgressRepo => courseProgressRepo.Insert(It.IsAny<CourseProgress>()))
                .Returns(Task.CompletedTask);

            var mockCourseRepo = new Mock<ICourseRepository>();
            mockCourseRepo
                .Setup(courseRepo => courseRepo.Get(course.Id))
                .Returns(Task.FromResult(course));

            var result = new UsersController(
                mockCoursePrgressRepo.Object,
                mockUserRepo.Object,
                mockCourseRepo.Object) {
                ControllerContext = { HttpContext = Utilits.CreateContext("http", "host", "path") }
            };

            return result;
        }

        private UsersController CreatePostUserTestUsersController()
        {
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.Insert(user))
                .Returns(Task.CompletedTask);

            mockUserRepo.Setup(repo => repo.Contains(It.IsAny<int>()))
                .Returns(Task.FromResult(false));

            var result = new UsersController(null, mockUserRepo.Object, null) {
                ControllerContext = { HttpContext = Utilits.CreateContext("http", "host", "path") }
            };

            return result;
        }

        private UsersController CreateGetTestUserController()
        {
            var mockCoureseProgressRepo = new Mock<ICourseProgressRepository>();
            mockCoureseProgressRepo.Setup(repo => repo.Get(user.Id, course.Id))
                .Returns(Task.FromResult(courseProgress));

            var result = new UsersController(mockCoureseProgressRepo.Object, null, null) {
                ControllerContext = { HttpContext = Utilits.CreateContext("http", "host", "path") }
            };

            return result;
        }

        private UsersController CreatePostAnswerTestUsersController()
        {
            var mockCoureseProgressRepo = new Mock<ICourseProgressRepository>();
            mockCoureseProgressRepo.Setup(repo => repo.Get(user.Id, course.Id))
                .Returns(Task.FromResult(courseProgress));

            var mockCourseRepo = new Mock<ICourseRepository>();
            mockCourseRepo
                .Setup(courseRepo => courseRepo.Get(course.Id))
                .Returns(Task.FromResult(course));
            
            var result = new UsersController(mockCoureseProgressRepo.Object, null, mockCourseRepo.Object) {
                ControllerContext = { HttpContext = Utilits.CreateContext("http", "host", "path") }
            };
           
            return result;
        }
    }
}

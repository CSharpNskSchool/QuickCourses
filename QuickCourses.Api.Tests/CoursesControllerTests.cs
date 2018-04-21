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
            course = new Course {
                Id = ObjectId.GenerateNewId().ToString(),
                Description = new Description { Name = "Test Course", Overview = "Course to test Api" },
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
                                        CorrectAnswers = new List<int> {0}
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var mockRepo = new Mock<ICourseRepository>();
            mockRepo
                .Setup(repo => repo.GetAll())
                .Returns(Task.FromResult((IEnumerable<Course>) new[] {course}));

            mockRepo
                .Setup(repo => repo.Get(course.Id))
                .Returns(Task.FromResult(course));

            controller = new CoursesController(mockRepo.Object);
        }

        [Test]
        public void GetAllCoursesTest()
        {
            var response = controller.GetAllCourses().Result;

            Utilits.CheckResponseValue<OkObjectResult, IEnumerable<Course>>(response, new[] {course});
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
    }
}

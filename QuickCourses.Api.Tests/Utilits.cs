using System.Collections.Generic;
using System.Security.Claims;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using QuickCourses.Models.Primitives;

namespace QuickCourses.Api.Tests
{
    public static class Utilits
    {
        public static void CheckResponseValue<TResponse, TValue>(IActionResult response, TValue expectedValue)
            where TResponse : ObjectResult
        {
            Assert.IsInstanceOf(typeof(TResponse), response);

            var value = ((TResponse)response).Value;

            var compareLogic = new CompareLogic();
            var compareResult = compareLogic.Compare(expectedValue, value);

            Assert.IsTrue(compareResult.AreEqual);
        }

        public static HttpContext CreateContext(string scheme, string host, string path, ClaimsPrincipal user = null)
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(context => context.Request.Scheme).Returns(() => scheme);
            httpContext.Setup(context => context.Request.Host).Returns(() => new HostString(host));
            httpContext.Setup(context => context.Request.Path).Returns(() => new PathString($"/{path}"));
            httpContext.Setup(context => context.Request.Query).Returns(() => new QueryCollection());
            httpContext.Setup(context => context.User).Returns(() => user);

            return httpContext.Object;
        }

        public static Course CreateCourse()
        {
            return new Course
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
        }
    }
}

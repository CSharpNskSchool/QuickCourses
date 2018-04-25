using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using QuickCourses.Models.Primitives;
using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Newtonsoft.Json;
using QuickCourses.Models.Authentication;
using QuickCourses.Models.Progress;
using QuickCourses.Models.Interaction;

namespace QuickCourses.Client.Tests
{
    public class JsonComparer<T> : IEqualityComparer<T> where T :class
    {
        public bool Equals(T x, T y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var str1 = JsonConvert.SerializeObject(x);
            var str2 = JsonConvert.SerializeObject(y);

            return str1.Equals(str2);
        }

        public int GetHashCode(T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return JsonConvert.SerializeObject(obj).GetHashCode();
        }
    }

    public class QuickCoursesClientTests : IDisposable
    {
        private readonly TestServer server;
        private readonly QuickCoursesClient client;
        private readonly IEnumerable<Course> courses;
        private readonly Course firstCourse;
        private readonly Ticket ticket;

        public QuickCoursesClientTests()
        {
            this.server = new TestServer(new WebHostBuilder().UseStartup<Api.Startup>());
            this.client = new QuickCoursesClient(ApiVersion.V1, server.CreateClient());
            this.ticket = client.GetTicketAsync(new AuthData { Login = "bot", Password = "12345" }).Result;
            this.courses = client.GetCoursesAsync(ticket).Result;

            Assert.NotNull(courses);
            firstCourse = courses.FirstOrDefault();
            Assert.NotNull(firstCourse);
        }
        
        [Fact]
        public void Course_LoadCorrectly()
        {
            var courseFromId = client.GetCourseAsync(ticket, firstCourse.Id).Result;
            Assert.NotNull(courseFromId);
            Assert.Equal(firstCourse, courseFromId, new JsonComparer<Course>());
        }

        [Fact]
        public void Lesson_LoadCorrectly()
        {
            var firstLesson = firstCourse.Lessons.FirstOrDefault();
            Assert.NotNull(firstCourse);
            var lessonFromId = client.GetLessonAsync(ticket, firstLesson.CourseId.ToString(), firstLesson.Id).Result;
            Assert.Equal(firstLesson, lessonFromId, new JsonComparer<Lesson>());
        }

        [Fact]
        public void LessonStep_LoadCorrectly()
        {
            var firstLessonStep = firstCourse.Lessons.FirstOrDefault()?.Steps.FirstOrDefault();
            Assert.NotNull(firstCourse);
            var lessonStepFromId = client.GetLessonStepAsync(ticket, firstLessonStep.CourseId.ToString(), firstLessonStep.LessonId, firstLessonStep.Id).Result;
            Assert.Equal(firstLessonStep, lessonStepFromId, new JsonComparer<LessonStep>());
        }

        [Fact]
        public void User_RegisterAndGiveMakeAnswer_Easy()
        {
            var user = new User
            {
                Name = "Vasya",
                Password = "12345",
                Login = "Vasya227"
            };

            client.RegisterAsync(user).Wait();

            var userTicket = client.GetTicketAsync(ticket, user.Login).Result;

            client.StartCourseAsync(userTicket, firstCourse.Id);

            var result = client.SendAnswerAsync(userTicket,
                                                firstCourse.Id,
                                                0,
                                                0,
                                                new Answer
                                                {
                                                    QuestionId = 0,
                                                    SelectedAnswers = new List<int> { 0 }
                                                }).Result;

            AssertQuestionStateInProgrees_Like(userTicket, result);
        }

        [Fact]
        public void User_RegisterAndGiveMakeAnswer_Hard()
        {
            var user = new User
            {
                Name = "Vasya",
                Password = "12345",
                Login = "Vasya228"
            };

            client.RegisterAsync(user).Wait();

            var userTicket = client.GetTicketAsync(ticket, user.Login).Result;

            client.StartCourseAsync(userTicket, firstCourse.Id);

            var questionState1 = client.SendAnswerAsync(userTicket,
                                                firstCourse.Id,
                                                0,
                                                0,
                                                new Answer
                                                {
                                                    QuestionId = 0,
                                                    SelectedAnswers = new List<int> { 0 }
                                                }).Result;

            AssertQuestionStateInProgrees_Like(userTicket, questionState1);

            var questionState2 = client.SendAnswerAsync(userTicket,
                                                firstCourse.Id,
                                                1,
                                                0,
                                                new Answer
                                                {
                                                    QuestionId = 0,
                                                    SelectedAnswers = new List<int> { 0 }
                                                }).Result;

            AssertQuestionStateInProgrees_Like(userTicket, questionState2);
        }

        [Fact]
        public void Course_ThrowsWhen_BadArgrumnts()
        {
            Assert.ThrowsAsync<ArgumentException>(()=>client.GetCourseAsync(ticket, "im smart client give me my items"));
        }
        [Fact]
        public void Client_ThrowsWhen_NullApiUrl_And_NullApiVer()
        {
            Assert.Throws<ArgumentNullException>(() => new QuickCoursesClient(ApiVersion.V1, default(string)));
        }

        private void AssertQuestionStateInProgrees_Like(Ticket userTicket, QuestionState questionState)
        {
            var progress = client.GetCourseProgressAsync(userTicket, questionState.CourseId).Result;
            Assert.NotNull(progress.LessonProgresses);

            var firstLessonProgress = progress.LessonProgresses.FirstOrDefault(x => x.LessonId == questionState.LessonId);
            Assert.NotNull(firstLessonProgress);

            var firstStepProgress = firstLessonProgress.LessonStepProgress.FirstOrDefault(x => x.StepId == questionState.StepId);
            Assert.NotNull(firstStepProgress);

            var firstQuestionState = firstStepProgress.QuestionStates.FirstOrDefault(x =>x.QuestionId == questionState.QuestionId);
            Assert.NotNull(firstQuestionState);

            Assert.Equal(firstQuestionState, questionState, new JsonComparer<QuestionState>());
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}

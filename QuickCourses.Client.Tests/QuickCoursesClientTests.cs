using QuickCourses.Models.Primitives;
using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Newtonsoft.Json;
using QuickCourses.Models.Authentication;
using QuickCourses.Models.Progress;
using QuickCourses.Models.Interaction;
using QuickCourses.TestHelper;

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
        private readonly QuickCoursesTestServer server;
        private readonly QuickCoursesClient client;
        private readonly IEnumerable<Course> courses;
        private readonly Course firstCourse;
        private readonly Ticket ticket;

        public QuickCoursesClientTests()
        {
            server = new QuickCoursesTestServer();
            client = new QuickCoursesClient(ApiVersion.V1, server.CreateClient());

            server.UseUsers(TestUsers.CreateSuperUserSample(), TestUsers.CreateUserSample());
            server.UseCourses(TestCourses.CreateBasicSample());

            ticket = client.GetTicketAsync(new AuthData { Login = "bot", Password = "12345" }).Result;
            courses = client.GetCoursesAsync(ticket).Result;
            firstCourse = courses.First();
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

            client.StartCourseAsync(userTicket, firstCourse.Id).Wait();

            var result = client.SendAnswerAsync(
                userTicket,
                firstCourse.Id,
                lessonId: 0,
                stepId: 0,
                answer: new Answer
                {
                    QuestionId = 0,
                    SelectedAnswers = new List<int> { 0 }
                }).Result;

            AssertQuestionStateInProgrees_Like(userTicket, result);
        }

        [Fact]
        public void User_RegisterAndGiveAnswer_Hard()
        {
            var user = new User
            {
                Name = "Vasya",
                Password = "12345",
                Login = "Vasya228"
            };

            client.RegisterAsync(user).Wait();

            var userTicket = client.GetTicketAsync(ticket, user.Login).Result;

            client.StartCourseAsync(userTicket, firstCourse.Id).Wait();

            var questionState1 = client.SendAnswerAsync(
                userTicket,
                firstCourse.Id,
                lessonId: 0,
                stepId: 0,
                answer: new Answer
                {
                    QuestionId = 0,
                    SelectedAnswers = new List<int> { 0 }
                }).Result;

            AssertQuestionStateInProgrees_Like(userTicket, questionState1);

            //Не знаю, что здесь должно было быть, но в курсе нет урока с индексом 1
            //var questionState2 = client.SendAnswerAsync(
            //    userTicket,
            //    firstCourse.Id,
            //    lessonId: 1,
            //    stepId: 0,
            //    answer: new Answer
            //    {
            //        QuestionId = 0,
            //        SelectedAnswers = new List<int> { 0 }
            //    }).Result;

            //AssertQuestionStateInProgrees_Like(userTicket, questionState2);
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

            var firstStepProgress = firstLessonProgress
                                        .StepProgresses
                                        .FirstOrDefault(x => x.StepId == questionState.StepId);
            Assert.NotNull(firstStepProgress);

            var firstQuestionState = firstStepProgress
                                        .QuestionStates
                                        .FirstOrDefault(x =>x.QuestionId == questionState.QuestionId);
            Assert.NotNull(firstQuestionState);

            Assert.Equal(firstQuestionState, questionState, new JsonComparer<QuestionState>());
        }

        public void Dispose()
        {
            client.Dispose();
            server.Dispose();
        }
    }
}

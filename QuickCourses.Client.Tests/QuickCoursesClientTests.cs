using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Newtonsoft.Json;
using QuickCourses.Api.Models.Authentication;
using QuickCourses.Api.Models.Interaction;
using QuickCourses.Api.Models.Primitives;
using QuickCourses.Api.Models.Progress;
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
            client = new QuickCoursesClient(ApiVersion.V1, "http://api", server.CreateClient());

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
            var lessonFromId = client.GetLessonAsync(ticket, firstLesson.CourseId, firstLesson.Id).Result;
            Assert.Equal(firstLesson, lessonFromId, new JsonComparer<Lesson>());
        }

        [Fact]
        public void LessonStep_LoadCorrectly()
        {
            var firstLessonStep = firstCourse.Lessons.FirstOrDefault()?.Steps.FirstOrDefault();
            Assert.NotNull(firstCourse);
            var lessonStepFromId = client.GetLessonStepAsync(
                ticket, 
                firstLessonStep.CourseId, 
                firstLessonStep.LessonId, 
                firstLessonStep.Id).Result;
            
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

            var progress = client.StartCourseAsync(userTicket, firstCourse.Id).Result;

            var result = client.SendAnswerAsync(
                userTicket,
                progress.Id,
                lessonId: 0,
                stepId: 0,
                answer: new Answer
                {
                    QuestionId = 0,
                    SelectedAnswers = new List<int> {0}
                }).Result;

            AssertQuestionStateInProgrees_Like(userTicket, result);
        }

        [Fact]
        public async void Course_ThrowsWhen_BadArgrumnts()
        {
            await Assert.ThrowsAsync<KeyNotFoundException>(() => client.GetCourseAsync(ticket, "im smart client give me my items"));
        }
        
        [Fact]
        public void Client_ThrowsWhen_NullApiUrl_And_NullApiVer()
        {
            Assert.Throws<ArgumentNullException>(() => new QuickCoursesClient(ApiVersion.V1, default(string)));
        }

        private void AssertQuestionStateInProgrees_Like(Ticket userTicket, QuestionState questionState)
        {
            var progress = client.GetCourseProgressAsync(userTicket, questionState.ProgressId).Result;
            Assert.NotNull(progress.LessonProgresses);

            var firstLessonProgress = progress.LessonProgresses.FirstOrDefault(x => x.LessonId == questionState.LessonId);
            Assert.NotNull(firstLessonProgress);

            var firstStepProgress = firstLessonProgress
                                        .StepProgresses
                                        .FirstOrDefault(x => x.Id == questionState.StepId);
            
            Assert.NotNull(firstStepProgress);

            var firstQuestionState = firstStepProgress
                                        .QuestionStates
                                        .FirstOrDefault(x =>x.QuestionId == questionState.QuestionId);
            
            Assert.NotNull(firstQuestionState);

            Assert.Equal(firstQuestionState, questionState, new JsonComparer<QuestionState>());
        }

        [Fact]
        public void GetIdByLogin_Test()
        {
            var user = TestUsers.CreateUserSample();

            var result = client.GetIdByLoginAsync(ticket, user.Login).Result;
            
            Assert.True(!string.IsNullOrEmpty(result));
        }
        
        public void Dispose()
        {
            client.Dispose();
            server.Dispose();
        }
    }
}

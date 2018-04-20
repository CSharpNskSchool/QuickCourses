using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using QuickCourses.Models.Primitives;
using System;
using System.Linq;
using System.Collections.Generic;
using Xunit;
using Newtonsoft.Json;
using QuickCourses.Models.Interaction;
using QuickCourses.Models.Progress;

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

            if (x == null)
            {
                throw new ArgumentNullException(nameof(x));
            }

            if (y == null)
            {
                throw new ArgumentNullException(nameof(y));
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

    public class ClientTests : IDisposable
    {
        private readonly TestServer server;
        private readonly QuickCoursesClient client;
        private readonly IEnumerable<Course> courses;
        private readonly Course firstCourse;

        public ClientTests()
        {
            server = new TestServer(new WebHostBuilder().UseStartup<Api.Startup>());
            client = new QuickCoursesClient(ApiVersion.V0, server.CreateClient());
            courses = client.GetCoursesAsync().Result;
            Assert.NotNull(courses);
            firstCourse = courses.FirstOrDefault();
            Assert.NotNull(firstCourse);
        }
        
        [Fact]
        public void Course_LoadCorrectly()
        {
            var courseFromId = client.GetCourseAsync(firstCourse.Id).Result;
            Assert.NotNull(courseFromId);
            Assert.Equal(firstCourse, courseFromId, new JsonComparer<Course>());
        }

        [Fact]
        public void Lesson_LoadCorrectly()
        {
            var firstLesson = firstCourse.Lessons.FirstOrDefault();
            Assert.NotNull(firstCourse);
            var lessonFromId = client.GetLessonAsync(firstLesson.CourseId.ToString(), firstLesson.Id).Result;
            Assert.Equal(firstLesson, lessonFromId, new JsonComparer<Lesson>());
        }

        [Fact]
        public void LessonStep_LoadCorrectly()
        {
            var firstLessonStep = firstCourse.Lessons.FirstOrDefault()?.Steps.FirstOrDefault();
            Assert.NotNull(firstCourse);
            var lessonStepFromId = client.GetLessonStepAsync(firstLessonStep.CourseId.ToString(), firstLessonStep.LessonId, firstLessonStep.Id).Result;
            Assert.Equal(firstLessonStep, lessonStepFromId, new JsonComparer<LessonStep>());
        }

        [Fact]
        public void User_RegisterAndGiveMakeAnswer_Easy()
        {
            var user = new User
            {
                Id = 777,
                Name = "Vasya"
            };

            RegisterUser(user);
            UserGiveAnswer(user, new Answer
            {
                QuestionId = 0,
                SelectedAnswers = new List<int> { 0 }
            });
        }

        [Fact]
        public void User_RegisterAndGiveMakeAnswer_Hard()
        {
            var user = new User
            {
                Id = 999,
                Name = "Petya"
            };

            RegisterUser(user);
            UserGiveAnswer(user, new Answer()
            {
                QuestionId = 0,
                SelectedAnswers = new List<int> { 0 }
            });
            UserGiveAnswer(user, new Answer()
            {
                QuestionId = 1,
                SelectedAnswers = new List<int> { 0, 1 }
            });

        }

        [Fact]
        public void Course_ThrowsWhen_BadArgrumnts()
        {
            Assert.ThrowsAsync<ArgumentException>(()=>client.GetCourseAsync("im stuiped client give me please some course"));
        }
        [Fact]
        public void Client_ThrowsWhen_NullApiUrl_And_NullApiVer()
        {
            Assert.Throws<ArgumentNullException>(() => new QuickCoursesClient(null, string.Empty));
            Assert.Throws<ArgumentNullException>(() => new QuickCoursesClient(ApiVersion.V0, default(string)));

        }
        private void RegisterUser(User user)
        {
            client.RegisterUserAsync(user).Wait();
            client.StartUserCourseAsync(user.Id, firstCourse.Id).Wait();
        }

        private void UserGiveAnswer(User user, Answer answer)
        {
            var state = client.SendUserAnswerAsync(
              userId: user.Id,
              courseId: firstCourse.Id,
              lessonId: 0,
              stepId: 0,
              answer: new Answer
              {
                  QuestionId = 0,
                  SelectedAnswers = new List<int> { 0 }
              }).Result;
            
            var progress = client.GetUserCourseAsync(user.Id, firstCourse.Id).Result;

            Assert.NotNull(progress.LessonProgresses);
            
            var firstLessonProgress = progress.LessonProgresses.FirstOrDefault();
            Assert.NotNull(firstLessonProgress);
            
            var firstStepProgress = firstLessonProgress.LessonStepProgress.FirstOrDefault();
            Assert.NotNull(firstStepProgress);
            
            var firstQuestionState = firstStepProgress.QuestionStates.FirstOrDefault();
            Assert.NotNull(firstQuestionState);
            
            Assert.Equal(firstQuestionState, state, new JsonComparer<QuestionState>());
        }
        
        public void Dispose()
        {
            client.Dispose();
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Interaction;
using QuickCourses.Models.Progress;
using QuickCourses.Models.Errors;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("QuickCourses.Client.Tests")]
namespace QuickCourses.Client
{
    public class QuickCoursesClient : IQuickCoursesClient
    {
        private readonly HttpClient client;
        private readonly string apiUrl;
        private readonly string version;

        public QuickCoursesClient(ApiVersion apiVersion, string apiUrl)
        {
            this.version = apiVersion?.ToString() ?? throw new ArgumentNullException(nameof(apiVersion));
            this.apiUrl = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
            this.client = new HttpClient();
        }

        internal QuickCoursesClient(ApiVersion apiVersion, HttpClient client)
        {
            this.apiUrl = string.Empty;
            this.version = apiVersion?.ToString() ?? throw new ArgumentNullException(nameof(apiVersion));
            this.client = client;
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public Task<IEnumerable<Course>> GetCoursesAsync()
        {
            return InvokeApiMethod<IEnumerable<Course>>(HttpMethod.Get, "courses");
        }

        public Task<Description> GetCourseDescriptionAsync(string courseId)
        {
            return InvokeApiMethod<Description>(HttpMethod.Get, $"courses/{courseId}/description");
        }

        public Task<Course> GetCourseAsync(string courseId)
        {
            return InvokeApiMethod<Course>(HttpMethod.Get, $"courses/{courseId}");
        }

        public Task<IEnumerable<Lesson>> GetLessonsAsync(string courseId)
        {
            return InvokeApiMethod<IEnumerable<Lesson>>(HttpMethod.Get, $"courses/{courseId}/lessons");
        }

        public Task<Lesson> GetLessonAsync(string courseId, int lessonId)
        {
            return InvokeApiMethod<Lesson>(HttpMethod.Get, $"courses/{courseId}/lessons/{lessonId}");
        }

        public Task<IEnumerable<LessonStep>> GetLessonStepsAsync(string courseId, int lessonId) 
        {
            return InvokeApiMethod<IEnumerable<LessonStep>>(HttpMethod.Post,
                                                         $"courses/{courseId}/lessons/{lessonId}/steps");
        }

        public Task<LessonStep> GetLessonStepAsync(string courseId, int lessonId, int stepId)
        {
            return InvokeApiMethod<LessonStep>(HttpMethod.Get,
                                            $"courses/{courseId}/lessons/{lessonId}/steps/{stepId}");
        }

        public Task StartUserCourseAsync(string userId, string courseId)
        {
            var options = new CourseStartOptions {CourseId = courseId};

            return InvokeApiMethod(HttpMethod.Post, $"users/{userId}/courses", options);
        }

        public Task<LessonProgress> GetUserLessonAsync(int userId, string courseId, int lessonId)
        {
            return InvokeApiMethod<LessonProgress>(HttpMethod.Post,
                                                $"users/{userId}/courses/{courseId}/lessons/{lessonId}");
        }

        public Task<LessonStepProgress> GetUserLessonStepAsync(int userId, string courseId, int lessonId, int stepId)
        {
            return InvokeApiMethod<LessonStepProgress>(HttpMethod.Get,
                                          $"users/{userId}/courses/{courseId}/lessons/{lessonId}/steps/{stepId}");
        }

        public Task<CourseProgress> GetUserCourseAsync(int userId, string courseId)
        {
            return InvokeApiMethod<CourseProgress>(HttpMethod.Get, $"users/{userId}/courses/{courseId}");
        }

        public Task<QuestionState> SendUserAnswerAsync(int userId, string courseId, int lessonId, int stepId, Answer answer)
        {
            return InvokeApiMethod<QuestionState>(HttpMethod.Post,
                                               $"users/{userId}/courses/{courseId}/lessons/{lessonId}/steps/{stepId}",
                                               answer);
        }

        public Task<IEnumerable<CourseProgress>> GetUserCoursesAsync(int userId)
        {
            return InvokeApiMethod<IEnumerable<CourseProgress>>(HttpMethod.Get, $"users/{userId}/courses");
        }

        //public Task RegisterUserAsync(User user)
        //{
        //    return InvokeApiMethod(HttpMethod.Post, "users", user);
        //}

        private async Task InvokeApiMethod(HttpMethod httpMethod, string prefix, object content = null)
        {
            await GetResponse(httpMethod, prefix, content);
        }

        private async Task<T> InvokeApiMethod<T>(HttpMethod httpMethod, string prefix, object content = null)
        {
            var response = await GetResponse(httpMethod, prefix, content);
            
            var serializedObject = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(serializedObject);
        }

        private async Task<HttpResponseMessage> GetResponse(HttpMethod httpMethod, string prefix, object content)
        {
            var request = new HttpRequestMessage(httpMethod, $"{apiUrl}/api/{version}/{prefix}");

            if (content != null)
            {
                var serializedContent = JsonConvert.SerializeObject(content);
                request.Content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
            }

            var response = await client.SendAsync(request);

            if (HasError(response.StatusCode))
            {
                HandleError(response);
            }

            return response;
        }
     
        private bool HasError(HttpStatusCode httpStatusCode)
        {
            return httpStatusCode != HttpStatusCode.OK &&
                   httpStatusCode != HttpStatusCode.Accepted && 
                   httpStatusCode != HttpStatusCode.Created;
        }

        private void HandleError(HttpResponseMessage response)
        {
            var serializedObject = response.Content.ReadAsStringAsync().Result;
            var error = JsonConvert.DeserializeObject<Error>(serializedObject);

            if (error == null)
            {
                throw new Exception(Enum.GetName(typeof(HttpStatusCode), response.StatusCode));
            }

            switch (error.Code)
            {
                case Error.ErrorCode.BadArgument:
                    throw new ArgumentException(error.Message);
                default:
                    throw new Exception(error.Message);
            }
        }
    }
}

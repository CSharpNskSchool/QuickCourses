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
using QuickCourses.Models.Authentication;

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
            this.version = Enum.GetName(typeof(ApiVersion), apiVersion);
            this.apiUrl = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
            this.client = new HttpClient();
        }

        internal QuickCoursesClient(ApiVersion apiVersion, HttpClient client)
        {
            this.apiUrl = string.Empty;
            this.version = Enum.GetName(typeof(ApiVersion), apiVersion);
            this.client = client;
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public Task<Ticket> GetTicketAsync(Ticket ticket, string login)
        {
            return InvokeApiMethod<Ticket>(HttpMethod.Get, "auth", ticket, headers: new Dictionary<string, string> { ["Login"] =  login });
        }

        public Task<Ticket> GetTicketAsync(AuthData authData)
        {
            return InvokeApiMethod<Ticket>(HttpMethod.Post, "auth", content: authData);
        }

        public Task<IEnumerable<Course>> GetCoursesAsync(Ticket ticket)
        {
            return InvokeApiMethod<IEnumerable<Course>>(HttpMethod.Get, "courses", ticket);
        }

        public Task<Course> GetCourseAsync(Ticket ticket, string courseId)
        {
            return InvokeApiMethod<Course>(HttpMethod.Get, $"courses/{courseId}", ticket);
        }

        public Task<Description> GetCourseDescriptionAsync(Ticket ticket, string courseId)
        {
            return InvokeApiMethod<Description>(HttpMethod.Get, $"courses/{courseId}/dectrition", ticket);
        }

        public Task<IEnumerable<Lesson>> GetLessonsAsync(Ticket ticket, string courseId)
        {
            return InvokeApiMethod<IEnumerable<Lesson>>(HttpMethod.Get, $"courses/{courseId}/lessons", ticket);
        }

        public Task<Lesson> GetLessonAsync(Ticket ticket, string courseId, int lessonId)
        {
            return InvokeApiMethod<Lesson>(HttpMethod.Get, $"courses/{courseId}/lessons/{lessonId}", ticket);
        }

        public Task<IEnumerable<LessonStep>> GetLessonStepsAsync(Ticket ticket, string courseId, int lessonId)
        {
            return InvokeApiMethod<IEnumerable<LessonStep>>(HttpMethod.Get, $"courses/{courseId}/lessons/{lessonId}/steps", ticket);
        }

        public Task<LessonStep> GetLessonStepAsync(Ticket ticket, string courseId, int lessonId, int stepId)
        {
            return InvokeApiMethod<LessonStep>(HttpMethod.Get, $"courses/{courseId}/lessons/{lessonId}/steps/{stepId}", ticket);
        }

        public Task RegisterAsync(User user)
        {
            return InvokeApiMethod(HttpMethod.Post, "registration", content: user);
        }

        public Task StartCourseAsync(Ticket ticket, string courseId)
        {
            var startOptions = new CourseStartOptions
            {
                CourseId = courseId
            };

            return InvokeApiMethod<LessonStep>(HttpMethod.Post, "progress", ticket, content: startOptions);
        }

        public Task<LessonProgress> GetLessonProgressAsync(Ticket ticket, string courseId, int lessonId)
        {
            return InvokeApiMethod<LessonProgress>(HttpMethod.Get, $"progress/{courseId}/lessons/{lessonId}", ticket);
        }

        public Task<LessonStepProgress> GetLessonStepProgressAsync(Ticket ticket, string courseId, int lessonId, int stepId)
        {
            return InvokeApiMethod<LessonStepProgress>(HttpMethod.Get, $"progress/{courseId}/lessons/{lessonId}/steps/{stepId}", ticket);
        }

        public Task<CourseProgress> GetCourseProgressAsync(Ticket ticket, string courseId)
        {
            return InvokeApiMethod<CourseProgress>(HttpMethod.Get, $"progress/{courseId}", ticket);
        }

        public Task<QuestionState> SendAnswerAsync(Ticket ticket, string courseId, int lessonId, int stepId, Answer answer)
        {
            return InvokeApiMethod<QuestionState>(HttpMethod.Post, $"progress/{courseId}/lessons/{lessonId}/steps/{stepId}", ticket, content: answer);
        }

        public Task<IEnumerable<CourseProgress>> GetProgressAsync(Ticket ticket)
        {
            return InvokeApiMethod<IEnumerable<CourseProgress>>(HttpMethod.Post, $"progress", ticket);
        }

        private Task InvokeApiMethod(HttpMethod httpMethod, string path, Ticket ticket = null, object content = null, Dictionary<string, string> headers = null)
        {
            return MakeRequest(httpMethod, path, ticket, content, headers);
        }

        private async Task<T> InvokeApiMethod<T>(HttpMethod httpMethod, string path, Ticket ticket = null, object content = null, Dictionary<string, string> headers = null)
        {
            var response = await MakeRequest(httpMethod, path, ticket, content, headers);
            
            var serializedObject = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(serializedObject);
        }

        private async Task<HttpResponseMessage> MakeRequest(HttpMethod httpMethod, string path, Ticket ticket = null, object content = null, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(httpMethod, $"{apiUrl}/api/{version}/{path}");

            if (ticket != null)
            {
                if (!ticket.IsValid())
                {
                    throw new InvalidOperationException("Ticket is old");
                }

                request.Headers.Add("Authorization", "Bearer " + ticket.Source);
            }

            if (content != null)
            {
                var serializedContent = JsonConvert.SerializeObject(content);
                request.Content = new StringContent(serializedContent, Encoding.UTF8, "application/json");
            }

            if (headers != null)
            {
                foreach(var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await client.SendAsync(request);
            
            if (!response.IsSuccessStatusCode)
            {
                HandleError(response);
            }

            return response;
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
                case Error.ErrorCode.InvalidOperation:
                    throw new InvalidOperationException(error.Message);
                case Error.ErrorCode.NotFound:
                    throw new KeyNotFoundException(error.Message);
                default:
                    throw new Exception(error.Message);
            }
        }
    }
}

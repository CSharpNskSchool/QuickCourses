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
using System.Web;
using QuickCourses.Models.Authentication;

[assembly: InternalsVisibleTo("QuickCourses.Client.Tests")]
namespace QuickCourses.Client
{
    public class QuickCoursesClient : IQuickCoursesClient
    {
        private readonly HttpClient client;
        private readonly string apiUrl;
        private readonly string version;
        
        public QuickCoursesClient(ApiVersion apiVersion, string apiUrl, HttpClient client = null)
        {
            this.version = Enum.GetName(typeof(ApiVersion), apiVersion);
            this.apiUrl = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
            this.client = client ?? new HttpClient();
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public Task<Ticket> GetTicketAsync(Ticket ticket, string login)
        {
            return InvokeApiMethod<Ticket>(
                HttpMethod.Get,
                path: "auth",
                ticket: ticket,
                headers: new Dictionary<string, string> {["Login"] = login});
        }

        public Task<Ticket> GetTicketAsync(AuthData authData)
        {
            return InvokeApiMethod<Ticket>(
                HttpMethod.Post, 
                path: "auth", 
                content: authData);
        }

        public Task<IEnumerable<Course>> GetCoursesAsync(Ticket ticket)
        {
            return InvokeApiMethod<IEnumerable<Course>>(
                HttpMethod.Get, 
                path: "courses", 
                ticket: ticket);
        }

        public Task<Course> GetCourseAsync(Ticket ticket, string courseId)
        {
            return InvokeApiMethod<Course>(
                HttpMethod.Get, 
                path: $"courses/{courseId}", 
                ticket: ticket);
        }

        public Task<Description> GetCourseDescriptionAsync(Ticket ticket, string courseId)
        {
            return InvokeApiMethod<Description>(
                HttpMethod.Get, 
                path: $"courses/{courseId}/dectrition", 
                ticket: ticket);
        }

        public Task<IEnumerable<Lesson>> GetLessonsAsync(Ticket ticket, string courseId)
        {
            return InvokeApiMethod<IEnumerable<Lesson>>(
                HttpMethod.Get, 
                path: $"courses/{courseId}/lessons",
                ticket: ticket);
        }

        public Task<Lesson> GetLessonAsync(Ticket ticket, string courseId, int lessonId)
        {
            return InvokeApiMethod<Lesson>(
                HttpMethod.Get, 
                path: $"courses/{courseId}/lessons/{lessonId}",
                ticket: ticket);
        }

        public Task<IEnumerable<LessonStep>> GetLessonStepsAsync(Ticket ticket, string courseId, int lessonId)
        {
            return InvokeApiMethod<IEnumerable<LessonStep>>(
                HttpMethod.Get, 
                path: $"courses/{courseId}/lessons/{lessonId}/steps", 
                ticket: ticket);
        }

        public Task<LessonStep> GetLessonStepAsync(Ticket ticket, string courseId, int lessonId, int stepId)
        {
            return InvokeApiMethod<LessonStep>(
                HttpMethod.Get,
                path: $"courses/{courseId}/lessons/{lessonId}/steps/{stepId}",
                ticket: ticket);
        }

        public Task RegisterAsync(User user)
        {
            return InvokeApiMethod(
                HttpMethod.Post,
                path: "registration",
                content: user);
        }

        public Task<Progress> StartCourseAsync(Ticket ticket, string courseId, string userId = null)
        {
            var startOptions = new CourseStartOptions
            {
                CourseId = courseId
            };

            var query = userId != null ? new Dictionary<string, string> {["userId"] = userId} : null;
            
            return InvokeApiMethod<Progress>(
                HttpMethod.Post, 
                path: "progress", 
                ticket: ticket, 
                content: startOptions,
                queryParameters: query);
        }

        public Task<LessonProgress> GetLessonProgressAsync(Ticket ticket, string progressId, int lessonId)
        {
            return InvokeApiMethod<LessonProgress>(
                HttpMethod.Get,
                path: $"progress/{progressId}/lessons/{lessonId}", 
                ticket: ticket);
        }

        public Task<StepProgress> GetLessonStepProgressAsync(
            Ticket ticket,
            string progressId,
            int lessonId,
            int stepId)
        {
            return InvokeApiMethod<StepProgress>(
                HttpMethod.Get, 
                path: $"progress/{progressId}/lessons/{lessonId}/steps/{stepId}",
                ticket: ticket);
        }

        public Task<Progress> GetCourseProgressAsync(Ticket ticket, string progressId)
        {
            return InvokeApiMethod<Progress>(
                HttpMethod.Get, 
                path: $"progress/{progressId}",
                ticket: ticket);
        }

        public Task<QuestionState> SendAnswerAsync(
            Ticket ticket,
            string progressId,
            int lessonId,
            int stepId,
            Answer answer)
        {
            return InvokeApiMethod<QuestionState>(
                HttpMethod.Post,
                path: $"progress/{progressId}/lessons/{lessonId}/steps/{stepId}",
                ticket: ticket,
                content: answer);
        }

        public Task<IEnumerable<Progress>> GetProgressAsync(Ticket ticket, string userId)
        {
            var query = userId != null ? new Dictionary<string, string> {["userId"] = userId} : null;
            
            return InvokeApiMethod<IEnumerable<Progress>>(
                HttpMethod.Post,
                path: $"progress",
                ticket: ticket,
                queryParameters: query);
        }

        private Task InvokeApiMethod(
            HttpMethod httpMethod,
            string path,
            Ticket ticket = null,
            object content = null,
            Dictionary<string, string> headers = null)
        {
            return MakeRequest(httpMethod, path, ticket, content, headers);
        }

        private async Task<T> InvokeApiMethod<T>(
            HttpMethod httpMethod,
            string path, 
            Ticket ticket = null,
            object content = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParameters = null)
        {
            var response = await MakeRequest(httpMethod, path, ticket, content, headers);

            var serializedObject = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(serializedObject);
        }

        private async Task<HttpResponseMessage> MakeRequest(
            HttpMethod httpMethod,
            string path,
            Ticket ticket = null,
            object content = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> queryParameters = null)
        {
            var uriBuilder = new UriBuilder($"{apiUrl}/api/{version}/{path}");

            if (queryParameters != null)
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                foreach (var parameter in queryParameters)
                {
                    query[parameter.Key] = parameter.Value;
                }

                uriBuilder.Query = query.ToString();
            }
            
            var request = new HttpRequestMessage(httpMethod, uriBuilder.Uri);

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
                foreach (var header in headers)
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

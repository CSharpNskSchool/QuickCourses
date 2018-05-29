using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using QuickCourses.Api.Models.Authentication;
using QuickCourses.Api.Models.Interaction;
using QuickCourses.Api.Models.Primitives;
using QuickCourses.Api.Models.Progress;
using QuickCourses.Client.Infrastructure;
using QuickCourses.Client.Interfaces;

[assembly: InternalsVisibleTo("QuickCourses.Client.Tests")]
namespace QuickCourses.Client.Clients
{
    public class QuickCoursesClient : ClientBase, IQuickCoursesClient
    {
        public QuickCoursesClient(ApiVersion apiVersion, string apiUrl, HttpClient client = null) 
            : base(apiVersion, apiUrl, client)
        {
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

        public Task RegisterAsync(RegistrationInfo registrationInfo)
        {
            return InvokeApiMethod(
                HttpMethod.Post,
                path: "users",
                content: registrationInfo);
        }

        public Task<CourseProgress> StartCourseAsync(Ticket ticket, string courseId, string userId = null)
        {
            var startOptions = new CourseStartOptions
            {
                CourseId = courseId
            };

            var query = userId != null ? new Dictionary<string, string> {["userId"] = userId} : null;
            
            return InvokeApiMethod<CourseProgress>(
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

        public Task<CourseProgress> GetCourseProgressAsync(Ticket ticket, string progressId)
        {
            return InvokeApiMethod<CourseProgress>(
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

        public Task<IEnumerable<CourseProgress>> GetProgressAsync(Ticket ticket, string userId)
        {
            var query = userId != null ? new Dictionary<string, string> {["userId"] = userId} : null;
            
            return InvokeApiMethod<IEnumerable<CourseProgress>>(
                HttpMethod.Get,
                path: "progress",
                ticket: ticket,
                queryParameters: query);
        }

        public Task<string> GetIdByLoginAsync(Ticket ticket, string login)
        {
            return InvokeApiMethod<string>(
                HttpMethod.Get,
                path: $"users/{login}/id",
                ticket: ticket);
        }
    }
}

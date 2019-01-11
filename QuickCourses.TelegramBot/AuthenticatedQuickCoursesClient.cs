using QuickCourses.Client;
using QuickCourses.Api.Models.Authentication;
using QuickCourses.Api.Models.Interaction;
using QuickCourses.Api.Models.Primitives;
using QuickCourses.Api.Models.Progress;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCourses.TelegramBot
{
    public class AuthenticatedQuickCoursesClient
    {
        private readonly QuickCoursesClient quickCoursesClient;
        private readonly Ticket ticket;

        public AuthenticatedQuickCoursesClient(AuthData auth, ApiVersion apiVersion, string apiUrl)
        {
            quickCoursesClient = new QuickCoursesClient(apiVersion, apiUrl);
            ticket = quickCoursesClient.GetTicketAsync(auth).Result;
        }
        public Task<IEnumerable<Course>> GetCoursesAsync()
        {
            return quickCoursesClient.GetCoursesAsync(ticket);
        }
        public Task<Course> GetCourseAsync(string courseId)
        {
            return quickCoursesClient.GetCourseAsync(ticket, courseId);
        }
        public Task<Description> GetCourseDescriptionAsync(string courseId)
        {
            return quickCoursesClient.GetCourseDescriptionAsync(ticket, courseId);
        }
        public Task<IEnumerable<Lesson>> GetLessonsAsync(string courseId)
        {
            return quickCoursesClient.GetLessonsAsync(ticket, courseId);
        }
        public Task<Lesson> GetLessonAsync(string courseId, int lessonId)
        {
            return quickCoursesClient.GetLessonAsync(ticket, courseId, lessonId);
        }
        public Task<IEnumerable<LessonStep>> GetLessonStepsAsync(string courseId, int lessonId)
        {
            return quickCoursesClient.GetLessonStepsAsync(ticket, courseId, lessonId);
        }

        public Task<LessonStep> GetLessonStepAsync(string courseId, int lessonId, int stepId)
        {
            return quickCoursesClient.GetLessonStepAsync(ticket, courseId, lessonId, stepId);
        }
        public Task RegisterAsync(RegistrationInfo user)
        {
            return quickCoursesClient.RegisterAsync(user);
        }
        public Task<CourseProgress> StartCourseAsync(string courseId, string userId)
        {
            return quickCoursesClient.StartCourseAsync(ticket, courseId, userId);
        }
        public Task<LessonProgress> GetLessonProgressAsync(string progressId, int lessonId)
        {
            return quickCoursesClient.GetLessonProgressAsync(ticket, progressId, lessonId);
        }
        public Task<StepProgress> GetLessonStepProgressAsync(string progressId, int lessonId, int stepId)
        {
            return quickCoursesClient.GetLessonStepProgressAsync(ticket, progressId, lessonId, stepId);
        }
        public Task<CourseProgress> GetCourseProgressAsync(string progressId)
        {
            return quickCoursesClient.GetCourseProgressAsync(ticket, progressId);
        }
        public Task<QuestionState> SendAnswerAsync(string progressId, int lessonId, int stepId, Answer answer)
        {
            return quickCoursesClient.SendAnswerAsync(ticket, progressId, lessonId, stepId, answer);
        }
        public Task<IEnumerable<CourseProgress>> GetProgressAsync(string userId)
        {
            return quickCoursesClient.GetProgressAsync(ticket, userId);
        }
        public Task<string> GetIdByLoginAsync(string login)
        {
            return quickCoursesClient.GetIdByLoginAsync(ticket, login);
        }
    }
}

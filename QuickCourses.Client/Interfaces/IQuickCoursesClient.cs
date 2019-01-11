using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using QuickCourses.Api.Models.Authentication;
using QuickCourses.Api.Models.Interaction;
using QuickCourses.Api.Models.Primitives;
using QuickCourses.Api.Models.Progress;

namespace QuickCourses.Client.Interfaces
{
    public interface IQuickCoursesClient : IDisposable
    {
        Task<Ticket> GetTicketAsync(Ticket ticket, string login);
        
        Task<Ticket> GetTicketAsync(AuthData authData);
        
        Task<IEnumerable<Course>> GetCoursesAsync(Ticket ticket);
        
        Task<Course> GetCourseAsync(Ticket ticket, string courseId);
        
        Task<Description> GetCourseDescriptionAsync(Ticket ticket, string courseId);
        
        Task<IEnumerable<Lesson>> GetLessonsAsync(Ticket ticket, string courseId);
        
        Task<Lesson> GetLessonAsync(Ticket ticket, string courseId, int lessonId);
        
        Task<IEnumerable<LessonStep>> GetLessonStepsAsync(Ticket ticket, string courseId, int lessonId);
        
        Task<LessonStep> GetLessonStepAsync(Ticket ticket, string courseId, int lessonId, int stepId);
        
        Task RegisterAsync(RegistrationInfo registrationInfo);
        
        Task<CourseProgress> StartCourseAsync(Ticket ticket, string userId, string courseId);
        
        Task<LessonProgress> GetLessonProgressAsync(Ticket ticket, string progressId, int lessonId);
        
        Task<StepProgress> GetLessonStepProgressAsync(Ticket ticket, string progressId, int lessonId, int stepId);
        
        Task<CourseProgress> GetCourseProgressAsync(Ticket ticket, string progressId);
        
        Task<QuestionState> SendAnswerAsync(Ticket ticket, string progressId, int lessonId, int stepId, Answer answer);
        
        Task<IEnumerable<CourseProgress>> GetProgressAsync(Ticket ticket, string userId);
        
        Task<string> GetIdByLoginAsync(Ticket ticket, string login);

    }
}

using MongoDB.Bson;
using QuickCourses.Models.Interaction;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Progress;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickCourses.Client
{
    //Возможно стоит переписать опираясь на SOL[I]D --> ISP
    public interface IQuickCoursesClient : IDisposable
    {
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task<Course> GetCourseAsync(string courseId);
        Task<Description> GetCourseDescriptionAsync(string courseId);
        Task<IEnumerable<Lesson>> GetLessonsAsync(string courseId);
        Task<Lesson> GetLessonAsync(string courseId, int lessonId);
        Task<IEnumerable<LessonStep>> GetLessonStepsAsync(string courseId, int lessonId);
        Task<LessonStep> GetLessonStepAsync(string courseId, int lessonId, int stepId);
        Task RegisterUserAsync(User user);
        Task StartUserCourseAsync(int userId, string courseId);
        Task<LessonProgress> GetUserLessonAsync(int userId, string courseId, int lessonId);
        Task<LessonStepProgress> GetUserLessonStepAsync(int userId, string courseId, int lessonId, int stepId);
        Task<CourseProgress> GetUserCourseAsync(int userId, string courseId);
        Task<QuestionState> SendUserAnswerAsync(int userId, string courseId, int lessonId, int stepId, Answer answer);
        Task<IEnumerable<CourseProgress>> GetUserCoursesAsync(int userId);
    }
}

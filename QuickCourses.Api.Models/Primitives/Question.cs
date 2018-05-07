using System.Collections.Generic;

namespace QuickCourses.Api.Models.Primitives
{
    public class Question
    {
        public string CourseId { get; set; }
        public int LessonId { get; set; }
        public int StepId { get; set; }
        public int Id { get; set; }
        public string Text { get; set; }
        public List<AnswerVariant> AnswerVariants { get; set; }
        public List<int> CorrectAnswers { get; set; }
        public int TotalAttemptsCount { get; set; }
    }
}

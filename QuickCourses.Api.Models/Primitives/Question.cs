using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuickCourses.Api.Models.Primitives
{
    public class Question
    {
        public string CourseId { get; set; }
        public int LessonId { get; set; }
        public int StepId { get; set; }
        public int Id { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public List<AnswerVariant> AnswerVariants { get; set; }
        [Required]
        public List<int> CorrectAnswers { get; set; }
        [Required]
        public int? TotalAttemptsCount { get; set; }
    }
}

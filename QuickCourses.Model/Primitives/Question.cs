using System.Collections.Generic;

namespace QuickCourses.Models.Primitives
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<AnswerVariant> AnswerVariants { get; set; }
        public List<int> CorrectAnswers { get; set; }
    }
}

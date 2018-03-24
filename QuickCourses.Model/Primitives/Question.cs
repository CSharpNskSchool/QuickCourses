using System.Collections.Generic;
using QuickCourses.Model.Interaction;

namespace QuickCourses.Model.Primitives
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<AnswerVariant> AnswerVariants { get; set; }
        public List<int> CorrectAnswers { get; set; }
    }
}

using System.Collections.Generic;
using QuickCourses.Model.Interaction;

namespace QuickCourses.Model.Primitives
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<Answer> Answers { get; set; }
        public List<Answer> CorrectAnswers { get; set; }
    }
}

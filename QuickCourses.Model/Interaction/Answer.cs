using System.Collections.Generic;

namespace QuickCourses.Models.Interaction
{
    public class Answer
    {
        public int QuestionId { get; set; }
        public List<int> SelectedAnswers { get; set; }
    }
}

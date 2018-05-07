using System.Collections.Generic;

namespace QuickCourses.Api.Models.Interaction
{
    public class Answer
    {
        public int QuestionId { get; set; }
        public List<int> SelectedAnswers { get; set; }
    }
}

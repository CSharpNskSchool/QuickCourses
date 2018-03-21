using System.Collections.Generic;

namespace QuickCourses.Model.Progress
{
    public class QuestionState
    {
        public List<int> CorrectlySelectedAnswers { get; set; }
        public List<int> SelectedAnswers { get; set; }
        public bool Passed { get; set; }
    }
}

using System.Collections.Generic;

namespace QuickCourses.Model.Progress
{
    public class LessonStepProgress
    {
        public int StepId { get; set; }
        public List<QuestionState> QuestionStates { get; set; }
        public bool Passed { get; set; }
    }
}

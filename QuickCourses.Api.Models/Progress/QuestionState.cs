using System.Collections.Generic;

namespace QuickCourses.Api.Models.Progress
{
    public class QuestionState
    {
        public string ProgressId { get; set; }
        public string CourseId { get; set; }
        public int LessonId { get; set; }
        public int StepId { get; set; }
        public int QuestionId { get; set; }
        public List<int> CorrectlySelectedAnswers { get; set; }
        public List<int> SelectedAnswers { get; set; }
        public bool Passed { get; set; }
        public int CurrentAttemptsCount { get; set; }
    }
}

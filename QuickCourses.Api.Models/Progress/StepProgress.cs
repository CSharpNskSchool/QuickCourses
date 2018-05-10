using System.Collections.Generic;

namespace QuickCourses.Api.Models.Progress
{
    public class StepProgress
    {
        public string ProgressId { get; set; }
        public string CourseId { get; set; }
        public int LessonId { get; set; }
        public int Id { get; set; }
        public List<QuestionState> QuestionStates { get; set; }
        public bool Passed { get; set; }
    }
}

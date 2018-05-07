using System.Collections.Generic;

namespace QuickCourses.Api.Models.Progress
{
    public class LessonProgress
    {
        public string ProgressId { get; set; }
        public string CourseId { get; set; }
        public int LessonId { get; set; }
        public List<StepProgress> StepProgresses { get; set; }
        public bool Passed { get; set; }
    }    
}

using System.Collections.Generic;

namespace QuickCourses.Model.Progress
{
    public class CourseProgress
    {
        public int CourceId { get; set; }
        public List<LessonProgress> LessonProgresses { get; set; }
        public bool Passed { get; set; }
    }
}

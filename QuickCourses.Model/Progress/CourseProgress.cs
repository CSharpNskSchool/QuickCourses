using System.Collections.Generic;

namespace QuickCourses.Models.Progress
{
    public class CourseProgress
    {
        public int CourceId { get; set; }
        public int UserId { get; set; }
        public List<LessonProgress> LessonProgresses { get; set; }
        public bool Passed { get; set; }
    }
}

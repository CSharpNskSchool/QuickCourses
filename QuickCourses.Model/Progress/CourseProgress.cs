using System.Collections.Generic;
using QuickCourses.Models.Interfaces;

namespace QuickCourses.Models.Progress
{
    public class CourseProgress : IValueWithId
    {
        public string Id { get; set; }
        public Statistics Statistics { get; set; }
        public string CourceId { get; set; }
        public string UserId { get; set; }
        public List<LessonProgress> LessonProgresses { get; set; }
        public bool Passed { get; set; }
    }
}

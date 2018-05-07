using System.Collections.Generic;

namespace QuickCourses.Api.Models.Progress
{
    public class CourseProgress
    {
        public string Id { get; set; }
        public Statistics Statistics { get; set; }
        public string CourceId { get; set; }
        public string UserId { get; set; }
        public List<LessonProgress> LessonProgresses { get; set; }
        public bool Passed { get; set; }
    }
}

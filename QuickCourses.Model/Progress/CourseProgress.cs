using System.Collections.Generic;
using MongoDB.Bson;

namespace QuickCourses.Models.Progress
{
    public class CourseProgress
    {
        public Statistics Statistics { get; set; }
        public string CourceId { get; set; }
        public string UserId { get; set; }
        public List<LessonProgress> LessonProgresses { get; set; }
        public bool Passed { get; set; }
    }
}

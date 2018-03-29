using System.Collections.Generic;
using MongoDB.Bson;

namespace QuickCourses.Models.Progress
{
    public class CourseProgress
    {
        public ObjectId CourceId { get; set; }
        public int UserId { get; set; }
        public List<LessonProgress> LessonProgresses { get; set; }
        public bool Passed { get; set; }
    }
}

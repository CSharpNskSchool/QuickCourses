using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Models.Progress
{
    public class LessonProgress
    {
        [BsonIgnore]
        public string CourseId { get; set; }
        public int LessonId { get; set; }
        public List<LessonStepProgress> LessonStepProgress { get; set; }
        public bool Passed { get; set; }
    }    
}

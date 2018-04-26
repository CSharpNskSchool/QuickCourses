using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Models.Progress
{
    public class CourseProgress
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public Statistics Statistics { get; set; }
        public string CourceId { get; set; }
        public string UserId { get; set; }
        public List<LessonProgress> LessonProgresses { get; set; }
        public bool Passed { get; set; }
    }
}

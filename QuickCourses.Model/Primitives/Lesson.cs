using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Models.Primitives
{
    public class Lesson
    {
        [BsonIgnore]
        public string CourseId { get; set; }
        public int Id { get; set; }
        public Description Description { get; set; }
        public List<LessonStep> Steps { get; set; }
    }
}

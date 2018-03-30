using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Models.Primitives
{
    public class LessonStep
    {
        [BsonIgnore]
        public ObjectId CourseId { get; set; }
        [BsonIgnore]
        public int LessonId { get; set; }
        public int Id { get; set; }
        public EducationalMaterial EducationalMaterial { get; set; }
        public List<Question> Questions { get; set; }
    }
}

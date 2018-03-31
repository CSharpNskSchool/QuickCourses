using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Models.Primitives
{
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public Description Description { get; set; }
        public List<Lesson> Lessons { get; set; }
    }
}

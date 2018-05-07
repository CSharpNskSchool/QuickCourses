using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Api.Data.Models.Primitives
{
    public class LessonData
    {
        [BsonIgnore]
        public string CourseId { get; set; }
        public int Id { get; set; }
        public DescriptionData DescriptionData { get; set; }
        public List<LessonStepData> Steps { get; set; }
    }
}

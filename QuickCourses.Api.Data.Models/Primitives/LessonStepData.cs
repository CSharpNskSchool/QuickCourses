using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Api.Data.Models.Primitives
{
    public class LessonStepData
    {
        [BsonIgnore]
        public string CourseId { get; set; }
        [BsonIgnore]
        public int LessonId { get; set; }
        public int Id { get; set; }
        public EducationalMaterialData EducationalMaterialData { get; set; }
        public List<QuestionData> Questions { get; set; }
    }
}

using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Api.Data.Models.Progress
{
    public class StepProgressData
    {
        [BsonIgnore]
        public string ProgressId { get; set; }
        [BsonIgnore]
        public string CourseId { get; set; }
        [BsonIgnore]
        public int LessonId { get; set; }
        public int Id { get; set; }
        public List<QuestionStateData> QuestionStates { get; set; }
        public bool Passed { get; set; }
    }
}

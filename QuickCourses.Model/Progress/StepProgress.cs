using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Models.Progress
{
    public class StepProgress
    {
        [BsonIgnore]
        public string CourseId { get; set; }
        [BsonIgnore]
        public int LessonId { get; set; }
        public int StepId { get; set; }
        public List<QuestionState> QuestionStates { get; set; }
        public bool Passed { get; set; }
    }
}

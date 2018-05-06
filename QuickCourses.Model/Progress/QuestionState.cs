using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Models.Progress
{
    public class QuestionState
    {
        [BsonIgnore]
        public string ProgressId { get; set; }
        [BsonIgnore]
        public string CourseId { get; set; }
        [BsonIgnore]
        public int LessonId { get; set; }
        [BsonIgnore]
        public int StepId { get; set; }
        public int QuestionId { get; set; }
        public List<int> CorrectlySelectedAnswers { get; set; }
        public List<int> SelectedAnswers { get; set; }
        public bool Passed { get; set; }
        public int CurrentAttemptsCount { get; set; }
    }
}

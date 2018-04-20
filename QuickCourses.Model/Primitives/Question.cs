using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Models.Primitives
{
    public class Question
    {
        [BsonIgnore]
        public string CourseId { get; set; }
        [BsonIgnore]
        public int LessonId { get; set; }
        [BsonIgnore]
        public int LessondStepId { get; set; }
        public int Id { get; set; }
        public string Text { get; set; }
        public List<AnswerVariant> AnswerVariants { get; set; }
        public List<int> CorrectAnswers { get; set; }
        public int TotalAttemptsCount { get; set; }
    }
}

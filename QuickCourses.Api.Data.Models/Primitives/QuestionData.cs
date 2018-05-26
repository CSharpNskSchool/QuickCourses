using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Api.Data.Models.Primitives
{
    public class QuestionData
    {
        [BsonIgnore]
        public string CourseId { get; set; }
        [BsonIgnore]
        public int LessonId { get; set; }
        [BsonIgnore]
        public int StepId { get; set; }
        public int Id { get; set; }
        public string Text { get; set; }
        public List<AnswerVariantData> AnswerVariants { get; set; }
        public List<int> CorrectAnswers { get; set; }
        public int? TotalAttemptsCount { get; set; }
    }
}

using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace QuickCourses.Api.Data.Models.Progress
{
    public class LessonProgressData
    {
        [BsonIgnore]
        public string ProgressId { get; set; }
        [BsonIgnore]
        public string CourseId { get; set; }
        public int LessonId { get; set; }
        public List<StepProgressData> StepProgresses { get; set; }
        public bool Passed { get; set; }
    }    
}

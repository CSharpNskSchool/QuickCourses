using System.Collections.Generic;
using QuickCourses.Api.Data.Models.Interfaces;

namespace QuickCourses.Api.Data.Models.Progress
{
    public class CourseProgressData : IIdentifiable
    {
        public string Id { get; set; }
        public StatisticsData StatisticsData { get; set; }
        public string CourceId { get; set; }
        public string UserId { get; set; }
        public List<LessonProgressData> LessonProgresses { get; set; }
        public bool Passed { get; set; }
    }
}

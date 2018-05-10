using System.Collections.Generic;

namespace QuickCourses.Api.Models.Primitives
{
    public class LessonStep
    {
        public string CourseId { get; set; }
        public int LessonId { get; set; }
        public int Id { get; set; }
        public EducationalMaterial EducationalMaterial { get; set; }
        public List<Question> Questions { get; set; }
    }
}

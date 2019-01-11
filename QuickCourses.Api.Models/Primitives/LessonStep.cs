using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuickCourses.Api.Models.Primitives
{
    public class LessonStep
    {
        public string CourseId { get; set; }
        public int LessonId { get; set; }
        public int Id { get; set; }
        [Required]
        public EducationalMaterial EducationalMaterial { get; set; }
        [Required]
        public List<Question> Questions { get; set; }
    }
}

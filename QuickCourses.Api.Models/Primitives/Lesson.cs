using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuickCourses.Api.Models.Primitives
{
    public class Lesson
    {
        public string CourseId { get; set; }
        public int Id { get; set; }
        [Required]
        public Description Description { get; set; }
        [Required]
        public List<LessonStep> Steps { get; set; }
    }
}

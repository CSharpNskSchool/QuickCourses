using System.Collections.Generic;

namespace QuickCourses.Api.Models.Primitives
{
    public class Lesson
    {
        public string CourseId { get; set; }
        public int Id { get; set; }
        public Description Description { get; set; }
        public List<LessonStep> Steps { get; set; }
    }
}

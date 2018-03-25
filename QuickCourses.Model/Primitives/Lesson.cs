using System.Collections.Generic;

namespace QuickCourses.Models.Primitives
{
    public class Lesson
    {
        public int Id { get; set; }
        public Description Description { get; set; }
        public List<LessonStep> Steps { get; set; }
    }
}

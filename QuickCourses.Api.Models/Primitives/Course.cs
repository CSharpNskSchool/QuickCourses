using System.Collections.Generic;

namespace QuickCourses.Api.Models.Primitives
{
    public class Course
    {
        public string Id { get; set; }
        public Description Description { get; set; }
        public List<Lesson> Lessons { get; set; }
    }
}

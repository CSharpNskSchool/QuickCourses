using System.Collections.Generic;
using QuickCourses.Models.Interfaces;

namespace QuickCourses.Models.Primitives
{
    public class Course : IValueWithId
    {
        public string Id { get; set; }
        public Description Description { get; set; }
        public List<Lesson> Lessons { get; set; }
    }
}

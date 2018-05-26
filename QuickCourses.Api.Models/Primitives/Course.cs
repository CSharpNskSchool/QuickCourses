using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuickCourses.Api.Models.Primitives
{
    public class Course
    {
        public string Id { get; set; }
        public string AuthorId { get; set; }
        public int Version { get; set; }
        [Required]
        public Description Description { get; set; }
        [Required]
        public List<Lesson> Lessons { get; set; }
    }
}

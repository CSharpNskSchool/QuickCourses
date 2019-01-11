using System.ComponentModel.DataAnnotations;

namespace QuickCourses.Api.Models.Primitives
{
    public class EducationalMaterial
    {
        [Required]
        public Description Description { get; set; }
        [Required]
        public string Article { get; set; }
    }
}

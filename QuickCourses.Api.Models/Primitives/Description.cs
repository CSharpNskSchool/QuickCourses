using System.ComponentModel.DataAnnotations;

namespace QuickCourses.Api.Models.Primitives
{
    public class Description
    {
        [Required]
        public string Name { get; set; }
        public string Overview { get; set; }
    }
}

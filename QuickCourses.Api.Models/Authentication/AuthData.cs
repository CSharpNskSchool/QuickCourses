using System.ComponentModel.DataAnnotations;

namespace QuickCourses.Api.Models.Authentication
{
    public class AuthData
    {
        public string Password { get; set; }
        [Required]
        public string Login { get; set; }
    }
}

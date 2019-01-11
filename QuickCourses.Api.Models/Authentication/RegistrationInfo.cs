using System;
using System.ComponentModel.DataAnnotations;

namespace QuickCourses.Api.Models.Authentication
{
    public class RegistrationInfo
    {
        public string Password { get; set; }
        [Required]
        public string Login { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime Born { get; set; }
    }
}

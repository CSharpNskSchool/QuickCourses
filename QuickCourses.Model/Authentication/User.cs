using System;
using QuickCourses.Models.Interfaces;

namespace QuickCourses.Models.Authentication
{
    public class User : IIdentifiable
    {
        public string Id { get; set; }
        public string Role { get; set; } 
        public string Password { get; set; }
        public string Login { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime Born { get; set; }
        public DateTime RegistrationTime { get; set; }
    }
}

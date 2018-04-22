using System;

namespace QuickCourses.Models.Authentication
{
    public class User // Данную модель нужно разбить на бизнес модель и публичную модель.
    {
        public string Role { get; set; } //Поле, которое должно быть скрытым
        public string Id { get; set; } //Поле, которое должно быть скрытым
        public Account Account { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime Born { get; set; }
        public DateTime RegistrationTime { get; set; }
    }
}

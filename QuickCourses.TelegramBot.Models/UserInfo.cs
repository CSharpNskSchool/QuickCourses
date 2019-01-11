using QuickCourses.Api.Data.Models.Interfaces;

namespace QuickCourses.TelegramBot.Models
{
    public class UserInfo : IIdentifiable
    {
        public string Id { get; set; }
        public UserAction CurrentAction { get; set; }
        public int TelegramId { get; set; }
        public CourseState CourseState { get; set; }
    }
}

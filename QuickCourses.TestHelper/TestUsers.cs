using QuickCourses.Models.Authentication;

namespace QuickCourses.TestHelper
{
    public static class TestUsers
    {
        public static User CreateSuperUserSample()
        {
            return new User
            {
                Login = "bot",
                Name = "bot",
                Password = "12345",
                Role = "Client"
            };
        }

        public static User CreateUserSample()
        {
            return new User
            {
                Name = "misha",
                Login = "mihail",
                Password = "sexbandit",
                Role = "User"
            };
        }
    }
}

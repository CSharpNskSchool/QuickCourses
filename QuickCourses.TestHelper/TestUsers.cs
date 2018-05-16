using QuickCourses.Api.Data.Models.Authentication;

namespace QuickCourses.TestHelper
{
    public static class TestUsers
    {
        public static UserData CreateSuperUserSample()
        {
            return new UserData
            {
                Login = "bot",
                Name = "bot",
                Password = "12345",
                Role = "Client"
            };
        }

        public static UserData CreateUserSample()
        {
            return new UserData
            {
                Name = "misha",
                Login = "mihail",
                Password = "sexbandit",
                Role = "User"
            };
        }
    }
}

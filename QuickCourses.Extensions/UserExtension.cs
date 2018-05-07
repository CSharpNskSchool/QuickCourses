using QuickCourses.Api.Data.Models.Authentication;
using QuickCourses.Api.Models.Authentication;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class UserExtension
    {
        public static User ToApiModel(this UserData userData)
        {
            var result = new User
            {
                Id = userData.Id,
                Role = userData.Role,
                Password = userData.Password,
                Login = userData.Password,
                Name = userData.Name,
                Surname = userData.Surname,
                Email = userData.Email,
                Born = userData.Born,
                RegistrationTime = userData.RegistrationTime
            };

            return result;
        }
    }
}

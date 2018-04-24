using System;
using System.Linq;
using System.Security.Claims;

namespace QuickCourses.Api.Extensions
{
    public static class UserExtension
    {
        public static string GetId(this ClaimsPrincipal user)
        {
            var result = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (result == null)
            {
                throw new ArgumentException("No id");
            }

            return result;
        }
    }
}

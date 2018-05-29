using System;
using System.Security.Claims;

namespace QuickCourses.Api.Extensions
{
    public static class ClaimsPrincipalExtension
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

        public static bool IsAuthor(this ClaimsPrincipal user, string courseId)
        {
            var userId = user.GetId();

            if (userId == null)
            {
                throw new ArgumentException("User hasn't id");
            }

            return courseId.StartsWith(userId);
        }
    }
}

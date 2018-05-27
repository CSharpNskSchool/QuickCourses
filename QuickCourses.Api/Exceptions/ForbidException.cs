using QuickCourses.Api.Models.Errors;

namespace QuickCourses.Api.Exceptions
{
    public class ForbidException : ApiException
    {
        public ForbidException(string message)
            : base(Error.ErrorCode.Forbidden, message)
        {
        }
    }
}

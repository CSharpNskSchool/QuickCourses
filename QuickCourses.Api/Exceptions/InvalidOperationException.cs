using QuickCourses.Api.Models.Errors;

namespace QuickCourses.Api.Exceptions
{
    public class InvalidOperationException : ApiException
    {
        public InvalidOperationException(string message) 
            : base(Error.ErrorCode.InvalidOperation, message)
        {
        }
    }
}

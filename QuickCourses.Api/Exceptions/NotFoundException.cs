using QuickCourses.Api.Models.Errors;

namespace QuickCourses.Api.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string message) 
            : base(Error.ErrorCode.NotFound, message)
        {
        }
    }
}

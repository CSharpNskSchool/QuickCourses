using System;
using QuickCourses.Api.Models.Errors;

namespace QuickCourses.Api.Exceptions
{
    public abstract class ApiException : Exception
    {
        public Error Error { get; }

        protected ApiException(Error.ErrorCode code, string message)
            : base(message)
        {
            Error = new Error
            {
                Code = code,
                Message = message
            };
        }
    }
}

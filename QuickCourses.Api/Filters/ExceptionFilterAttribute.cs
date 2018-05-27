using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuickCourses.Api.Exceptions;
using QuickCourses.Api.Models.Errors;

namespace QuickCourses.Api.Filters
{
    public class ExceptionFilterAttribute : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if(context.Exception is ApiException apiException)
            {
                switch (apiException.Error.Code)
                {
                    case Error.ErrorCode.BadArgument:
                    case Error.ErrorCode.InvalidOperation:
                        context.Result = new BadRequestObjectResult(apiException.Error);
                        break;
                    case Error.ErrorCode.NotFound:
                        context.Result = new NotFoundObjectResult(apiException.Error);
                        break;
                    case Error.ErrorCode.Forbidden:
                        context.Result = CreateResult(StatusCodes.Status403Forbidden, apiException.Error);
                        break;
                    case Error.ErrorCode.Internal:
                        context.Result = CreateResult(StatusCodes.Status500InternalServerError, apiException.Error);
                        break;
                }
            }
            else
            {
                var error = new Error
                {
                    Code = Error.ErrorCode.Internal,
                    Message = "Something went wrong"
                };

                context.Result = CreateResult(StatusCodes.Status500InternalServerError, error);
            }


            context.ExceptionHandled = true;
        }

        private IActionResult CreateResult(int statusCode, object value)
        {
            var result = new ObjectResult(value) {StatusCode = statusCode};
            return result;
        }
    }
}
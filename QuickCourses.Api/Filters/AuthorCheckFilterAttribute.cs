using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuickCourses.Api.Extensions;
using QuickCourses.Api.Models.Errors;

namespace QuickCourses.Api.Filters
{
    public class AuthorCheckFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!TryGetCourseId(context, out var courseId))
            {
                context.Result = new BadRequestObjectResult(new Error {
                    Code = Error.ErrorCode.BadArgument,
                    Message = "Invalid course id"
                });
            }

            var user = ((Controller)context.Controller).User;

            if (!user.IsAuthor(courseId))
            {
                var error = new Error {
                    Code = Error.ErrorCode.Forbidden,
                    Message = "User isn't course author"
                };

                context.Result = new ObjectResult(error) {StatusCode = StatusCodes.Status403Forbidden};
            }
        }

        private bool TryGetCourseId(ActionExecutingContext context, out string courseId)
        {
            courseId = null;

            if (!context.ActionArguments.TryGetValue("courseId", out var courseIdObject))
            {
                return false;
            }

            if (!(courseIdObject is string result))
            {
                return false;
            }

            courseId = result;
            return true;
        }
    }
}

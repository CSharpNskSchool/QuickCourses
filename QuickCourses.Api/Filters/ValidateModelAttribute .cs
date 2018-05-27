using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuickCourses.Api.Models.Errors;

namespace QuickCourses.Api.Filters
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(
                    new Error
                    {
                        Code = Error.ErrorCode.BadArgument,
                        Message = "Invalid state of argument"
                    }
                );
            }
        }
    }
}
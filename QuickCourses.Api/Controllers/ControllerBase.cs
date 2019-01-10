using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.Models.Errors;

namespace QuickCourses.Api.Controllers
{
    public class ControllerBase : Controller
    {
        protected IActionResult NotFound(string message)
        {
            var error = new Error
            {
                Code = Error.ErrorCode.NotFound,
                Message = message
            };
                
            return NotFound(error);
        }

        protected IActionResult BadRequest(string message)
        {
            var error = new Error
            {
                Code = Error.ErrorCode.BadArgument,
                Message = message
            };
                
            return BadRequest(error);
        }

        protected IActionResult InvalidOperation(string message)
        {
            var error = new Error
            {
                Code = Error.ErrorCode.InvalidOperation,
                Message = message
            };
                
            return BadRequest(error);
        }

        protected IActionResult Forbid(string message)
        {
            var error = new Error
            {
                Code = Error.ErrorCode.Forbidden,
                Message = message
            };

            return StatusCode(StatusCodes.Status403Forbidden, error);
        }
    }
}
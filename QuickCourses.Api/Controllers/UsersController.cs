using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Models.Authentication;

namespace QuickCourses.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/registration")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        public UsersController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody]User user)
        { 
            var login = user?.Login;

            if (string.IsNullOrEmpty(login))
            {
                return BadRequest("Invalid user object");
            }

            if (await userRepository.ContainsByLoginAsync(login))
            {
                return InvalidOperation($"User with login {login} already exists");
            }

            user.Role = "User";

            await userRepository.InsertAsync(user);

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}

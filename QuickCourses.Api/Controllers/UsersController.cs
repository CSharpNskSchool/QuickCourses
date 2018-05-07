using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Models.Authentication;

namespace QuickCourses.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/v1/users")]
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

            var userData = new Data.Models.Authentication.UserData
            {
                Born = user.Born,
                Email = user.Email,
                Login = user.Login,
                Name = user.Name,
                Password = user.Password,
                RegistrationTime = user.RegistrationTime,
                Role = "User"
            };

            await userRepository.InsertAsync(userData);

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("{login}/id")]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> GetId(string login)
        {
            var user = await userRepository.GetByLoginAsync(login);
            if (user == null)
            {
                return NotFound($"User with login = {login} doesnt exits");
            }

            return Ok(user.Id);
        }
    }
}

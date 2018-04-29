using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Models.Authentication;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace QuickCourses.Api.Controllers
{
    [Route("api/v1/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<User> userRepository;
        private readonly IConfigurationRoot configuration;
        private readonly JwtSecurityTokenHandler securityTokenHandler;

        public AuthController(IRepository<User> userRepository, IConfigurationRoot configuration)
        {
            this.securityTokenHandler = new JwtSecurityTokenHandler();
            this.configuration = configuration;
            this.userRepository = userRepository;
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Authentication([FromBody]AuthData authData)
        {
            if (authData == null)
            {
                return BadRequest("Bad authData.");
            }

            var user = await userRepository.Get(authData.Login);

            if (user == null)
            {
                return NotFound("AuthData not registered.");
            }

            if (user.Password != authData.Password)
            {
                return NotFound("Bad password or login.");
            }

            var ticketTime = user.Role == "Client" ? 
                            int.Parse(configuration["JasonWebToken:LifeTimeInMinutes:ForClient"]) :
                            int.Parse(configuration["JasonWebToken:LifeTimeInMinutes:ForUser"]);

            var result = GetTicket(user, ticketTime);

            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Authentication([FromHeader(Name = "Login")] string login)
        {
            if (login == null)
            {
                return BadRequest("No login.");
            }

            var user = await userRepository.Get(login);

            if (user == null)
            {
                return NotFound("User with this login not registered.");
            }
            
            var result = GetTicket(user, int.Parse(configuration["JasonWebToken:LifeTimeInMinutes:ForClient"]));

            return Ok(result);
        }

        private Ticket GetTicket(User user, int minutes)
        {
            var securityKey = GetSymmetricSecurityKey();
            var jwtToken = GetJwtSecurityToken(user, securityKey, minutes);
            var tokenSource = securityTokenHandler.WriteToken(jwtToken);

            return new Ticket
            {
                Source = tokenSource,
                ValidUntil = jwtToken.ValidTo
            };
        }

        private SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            var key = configuration["JasonWebToken:SecretKey"];
            var binnaryKey = Encoding.UTF8.GetBytes(key);

            return new SymmetricSecurityKey(binnaryKey);
        }

        private JwtSecurityToken GetJwtSecurityToken(User user, SymmetricSecurityKey securityKey, int minutes)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name ?? ""),
                new Claim(ClaimTypes.NameIdentifier, user.Id ?? ""),
                new Claim(ClaimTypes.Role, user.Role ?? "")
            };

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(configuration["JasonWebToken:Issuer"],
                                        configuration["JasonWebToken:Issuer"],
                                        claims: claims,
                                        expires: DateTime.Now.AddMinutes(minutes),
                                        signingCredentials: credentials);
        }
    }
}
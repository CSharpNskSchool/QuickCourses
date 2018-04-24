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
    [AllowAnonymous]
    [Route("api/v0/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IConfigurationRoot configuration;
        private readonly JwtSecurityTokenHandler securityTokenHandler;

        public AuthController(IUserRepository userRepository, IConfigurationRoot configuration)
        {
            this.securityTokenHandler = new JwtSecurityTokenHandler();
            this.configuration = configuration;
            this.userRepository = userRepository;
        }
        
        [HttpPost]
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

            var securityKey = GetSymmetricSecurityKey();
            var jwtToken = GetJwtSecurityToken(user, securityKey);
            var tokenSource = securityTokenHandler.WriteToken(jwtToken);
            var result = new Ticket
            {
                Source = tokenSource,
                Over = jwtToken.ValidTo
            };

            return Ok(result);
        }

        private SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            var key = configuration["JasonWebToken:SecretKey"];
            var binnaryKey = Encoding.UTF8.GetBytes(key);

            return new SymmetricSecurityKey(binnaryKey);
        }

        private JwtSecurityToken GetJwtSecurityToken(User user, SymmetricSecurityKey securityKey)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            return new JwtSecurityToken(configuration["JasonWebToken:Issuer"],
                                        configuration["JasonWebToken:Issuer"],
                                        claims: claims,
                                        expires: DateTime.Now.AddMinutes(
                                            int.Parse(configuration["JasonWebToken:LifeTimeMinutes"])),
                                        signingCredentials: credentials);
        }
    }
}
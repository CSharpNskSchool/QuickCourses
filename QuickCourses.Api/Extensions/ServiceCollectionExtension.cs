using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace QuickCourses.Api.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddJasonWebTokenAuth(
            this IServiceCollection services, 
            IConfigurationRoot configuration)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        LifetimeValidator = CustomLifetimeValidator,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["JasonWebToken:Issuer"],
                        ValidAudience = configuration["JasonWebToken:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["JasonWebToken:SecretKey"]))
                    };
                });
            return services;
        }
        private static bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
        {
            if (expires != null)
            {
                return expires > DateTime.UtcNow || notBefore == null;
            }
            return false;
        }
    }
}

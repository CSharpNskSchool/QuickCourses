using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Repositories;
using QuickCourses.Api.Extensions;

namespace QuickCourses.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            Settings = new Settings {
                ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value,
                Database = Configuration.GetSection("MongoConnection:Database").Value
            };
        }

        public IConfigurationRoot Configuration { get; }
        public Settings Settings { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(x => Configuration)
                .AddSingleton(x => Settings)
                .AddJasonWebTokenAuth(Configuration)
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<ICourseProgressRepository, CourseProgressRepository>()
                .AddScoped<ICourseRepository, CourseRepository>()
                .AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app
              .UseAuthentication()
              .UseMvc();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
        }
    }
}
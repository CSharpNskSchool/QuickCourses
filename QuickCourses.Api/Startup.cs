using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Repositories;
using QuickCourses.Api.Extensions;
using QuickCourses.Models.Authentication;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Progress;

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

            CourseRepositorySettings = new Settings(
                Configuration.GetSection("MongoConnection:Course:Database").Value,
                Configuration.GetSection("MongoConnection:Course:CollectionName").Value, 
                Configuration.GetSection("MongoConnection:Course:ConnectionString").Value);
            
            ProgressRepositorySettings = new Settings(
                Configuration.GetSection("MongoConnection:Progress:Database").Value,
                Configuration.GetSection("MongoConnection:Progress:CollectionName").Value,
                Configuration.GetSection("MongoConnection:Progress:ConnectionString").Value);
            
            UserRepositorySettings = new Settings(
                Configuration.GetSection("MongoConnection:User:Database").Value,
                Configuration.GetSection("MongoConnection:User:CollectionName").Value,
                Configuration.GetSection("MongoConnection:User:ConnectionString").Value);
        }

        public IConfigurationRoot Configuration { get; }
        private Settings CourseRepositorySettings { get; }
        private Settings ProgressRepositorySettings { get; }
        private Settings UserRepositorySettings { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(x => Configuration)
                .AddJasonWebTokenAuth(Configuration)
                .AddMvc();

            //возможно это нужно сделать как-то инче, но я не знаю как
            services
                .AddScoped<IRepository<Course>, RepositoryBase<Course>>(
                    _ => new CourseRepository(CourseRepositorySettings))
                .AddScoped<IRepository<CourseProgress>, RepositoryBase<CourseProgress>>(
                    _ => new ProgressRepository(ProgressRepositorySettings))
                .AddScoped<IRepository<User>, RepositoryBase<User>>(
                    _ => new UserRepository(UserRepositorySettings));
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
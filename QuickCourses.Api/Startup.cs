using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Data.Repositories;
using QuickCourses.Api.Extensions;
using QuickCourses.Api.Filters;

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
                connectionString: Configuration.GetSection("MongoConnection:Course:ConnectionString").Value,
                database: Configuration.GetSection("MongoConnection:Course:Database").Value, 
                collectionName: Configuration.GetSection("MongoConnection:Course:CollectionName").Value);
            
            ProgressRepositorySettings = new Settings(
                connectionString: Configuration.GetSection("MongoConnection:Progress:ConnectionString").Value,
                database: Configuration.GetSection("MongoConnection:Progress:Database").Value,
                collectionName: Configuration.GetSection("MongoConnection:Progress:CollectionName").Value);
            
            UserRepositorySettings = new Settings(
                connectionString: Configuration.GetSection("MongoConnection:User:ConnectionString").Value,
                database: Configuration.GetSection("MongoConnection:User:Database").Value,
                collectionName: Configuration.GetSection("MongoConnection:User:CollectionName").Value);
        }

        public IConfigurationRoot Configuration { get; }
        public IServiceCollection Services { get; }
        private Settings CourseRepositorySettings { get; }
        private Settings ProgressRepositorySettings { get; }
        private Settings UserRepositorySettings { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton(x => Configuration)
                .AddJasonWebTokenAuth(Configuration);
            
            services
                .AddMvc(options =>
                    {
                        options.Filters.Add(typeof(ValidateModelAttribute));
                        options.Filters.Add(typeof(ExceptionFilterAttribute));
                    }
                );
            
            services
                .AddSingleton<ICourseRepository>(new CourseRepository(CourseRepositorySettings))
                .AddSingleton<IProgressRepository>(new ProgressRepository(ProgressRepositorySettings))
                .AddSingleton<IUserRepository>(new UserRepository(UserRepositorySettings));
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
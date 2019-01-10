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
using Swashbuckle.AspNetCore.Swagger;

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
                .AddJasonWebTokenAuth(Configuration)
                .AddMvc();

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            services
                .AddSingleton<IRepository<CourseData>>(new CourseRepository(CourseRepositorySettings))
                .AddSingleton<IProgressRepository>(new ProgressRepository(ProgressRepositorySettings))
                .AddSingleton<IUserRepository>(new UserRepository(UserRepositorySettings));
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app
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
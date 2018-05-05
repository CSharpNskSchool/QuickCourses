using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using QuickCourses.Api;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Models.Primitives;
using System;
using System.Net.Http;
using QuickCourses.Models.Authentication;

namespace QuickCourses.TestHelper
{
    public class QuickCoursesTestServer : IDisposable
    {
        private readonly TestServer server;
        private readonly IRepository<Course> courseeRepository;
        private readonly IUserRepository userRepository;
        private readonly IProgressRepository progressRepository;

        public QuickCoursesTestServer()
        {
            server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            courseeRepository = (IRepository<Course>)server.Host.Services.GetService(typeof(IRepository<Course>));
            userRepository = (IUserRepository)server.Host.Services.GetService(typeof(IUserRepository));
            progressRepository = (IProgressRepository)server.Host.Services.GetService(typeof(IProgressRepository));
        }

        public TestServer Server => server;
        public Uri BaseAddress => server.BaseAddress;
        public HttpClient CreateClient()
        {
            return server.CreateClient();
        }

        public void UseCourses(params Course[] courses)
        {
            foreach(var course in courses)
            {
                courseeRepository.InsertAsync(course).Wait();
            }
        }

        public void UseUsers(params User[] users)
        {
            foreach(var user in users)
            {
                userRepository.InsertAsync(user).Wait();
            }
        }

        public void Dispose()
        {
            foreach(var user in userRepository.GetAllAsync().Result)
            {
                userRepository.DeleteAsync(user.Id).Wait();
            }

            foreach(var course in courseeRepository.GetAllAsync().Result)
            {
                courseeRepository.DeleteAsync(course.Id).Wait();
            }

            foreach (var progress in progressRepository.GetAllAsync().Result)
            {
                progressRepository.DeleteAsync(progress.Id);
            }
              
            server.Dispose();
        }
    }
}

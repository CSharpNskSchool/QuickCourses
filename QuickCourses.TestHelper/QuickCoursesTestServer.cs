using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using QuickCourses.Api;
using QuickCourses.Api.Data.DataInterfaces;
using System;
using System.Net.Http;
using QuickCourses.Api.Data.Models.Authentication;
using QuickCourses.Api.Data.Models.Primitives;

namespace QuickCourses.TestHelper
{
    public class QuickCoursesTestServer : IDisposable
    {
        private readonly TestServer server;
        private readonly IRepository<CourseData> coursesRepository;
        private readonly IUserRepository userRepository;
        private readonly IProgressRepository progressRepository;

        public QuickCoursesTestServer()
        {
            server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            coursesRepository = (ICourseRepository)server.Host.Services.GetService(typeof(ICourseRepository));
            userRepository = (IUserRepository)server.Host.Services.GetService(typeof(IUserRepository));
            progressRepository = (IProgressRepository)server.Host.Services.GetService(typeof(IProgressRepository));
        }

        public TestServer Server => server;
        public Uri BaseAddress => server.BaseAddress;
        public HttpClient CreateClient()
        {
            return server.CreateClient();
        }

        public void UseCourses(params CourseData[] coursesData)
        {
            foreach(var course in coursesData)
            {
                course.Id = coursesRepository.GenerateNewId();
                coursesRepository.InsertAsync(course).Wait();
            }
        }

        public void UseUsers(params UserData[] usersData)
        {
            foreach(var user in usersData)
            {
                user.Id = userRepository.GenerateNewId();
                userRepository.InsertAsync(user).Wait();
            }
        }

        public void Dispose()
        {
            foreach(var user in userRepository.GetAllAsync().Result)
            {
                userRepository.DeleteAsync(user.Id).Wait();
            }

            foreach(var course in coursesRepository.GetAllAsync().Result)
            {
                coursesRepository.DeleteAsync(course.Id).Wait();
            }

            foreach (var progress in progressRepository.GetAllAsync().Result)
            {
                progressRepository.DeleteAsync(progress.Id);
            }
              
            server.Dispose();
        }
    }
}

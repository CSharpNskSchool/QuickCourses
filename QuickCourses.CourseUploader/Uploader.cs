using System;
using System.IO;
using Newtonsoft.Json;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Models.Primitives;

namespace QuickCourses.CourseUploader
{
    public class Uploader
    {
        private readonly IRepository<Course> repository;

        public Uploader(IRepository<Course> repository)
        {
            this.repository = repository;
        }

        public void Upload(string path)
        {
            try
            {
                var serialized = File.ReadAllText(path);
                var course = JsonConvert.DeserializeObject<Course>(serialized);
                repository.InsertAsync(course).Wait();
                Console.WriteLine($"Uploaded successfully: {path}");
            }
            catch (Exception ex)
            {
                Console.Write($"Error: {ex}");
            }
        }

        public void Upload(string[] paths)
        {
            foreach (var path in paths)
            {
                Upload(path);
            }
        }
    }
}

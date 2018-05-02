using System;
using System.IO;
using Newtonsoft.Json;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Models.Interfaces;

namespace QuickCourses.CourseUploader
{
    public class Uploader<TValue>
        where TValue : IValueWithId
    {
        private readonly IRepository<TValue> repository;

        public Uploader(IRepository<TValue> repository)
        {
            this.repository = repository;
        }

        public void Upload(string path)
        {
            try
            {
                var serialized = File.ReadAllText(path);
                var value = JsonConvert.DeserializeObject<TValue>(serialized);
                repository.InsertAsync(value).Wait();
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

using System;
using System.IO;
using Newtonsoft.Json;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Repositories;
using QuickCourses.Models.Primitives;

namespace QuickCourses.CourseUploader
{
    public class Program
    {
        public static void Main(string[] paths)
        {
            try
            {
                var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("config.json"));
                var repository = new RepositoryBase<Course>(settings);
                var uploader = new Uploader<Course>(repository);
                uploader.Upload(paths);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

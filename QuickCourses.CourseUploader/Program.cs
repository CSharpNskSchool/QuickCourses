using System;
using System.IO;
using Newtonsoft.Json;
using QuickCourses.Api.Data.Infrastructure;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Data.Repositories;

namespace QuickCourses.CourseUploader
{
    public class Program
    {
        public static void Main(string[] paths)
        {
            try
            {
                var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("config.json"));
                var repository = new RepositoryBase<CourseData>(settings);
                var uploader = new Uploader<CourseData>(repository);
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

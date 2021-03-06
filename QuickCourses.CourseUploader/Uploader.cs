﻿using System;
using System.IO;
using Newtonsoft.Json;
using QuickCourses.Api.Data.DataInterfaces;
using QuickCourses.Api.Data.Models.Interfaces;

namespace QuickCourses.CourseUploader
{
    public class Uploader<TValue>
        where TValue : IIdentifiable
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
                Console.WriteLine($"Error: {ex}");
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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MongoDB.Bson;

namespace QuickCourses.Models.Primitives
{
    public class Course
    {
        public ObjectId Id { get; set; }
        public Description Description { get; set; }
        public List<Lesson> Lessons { get; set; }

        public static implicit operator Course(ConcurrentDictionary<int, Course> v)
        {
            throw new NotImplementedException();
        }
    }
}

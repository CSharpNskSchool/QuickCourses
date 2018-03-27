﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace QuickCourses.Models.Primitives
{
    public class Course
    {
        public int Id { get; set; }
        public Description Description { get; set; }
        public List<Lesson> Lessons { get; set; }

        public static implicit operator Course(ConcurrentDictionary<int, Course> v)
        {
            throw new NotImplementedException();
        }
    }
}
﻿using System.Collections.Generic;
using QuickCourses.Api.Data.Models.Interfaces;

namespace QuickCourses.Api.Data.Models.Primitives
{
    public class CourseData : IIdentifiable
    {
        public string Id { get; set; }
        public DescriptionData DescriptionData { get; set; }
        public List<LessonData> Lessons { get; set; }
    }
}

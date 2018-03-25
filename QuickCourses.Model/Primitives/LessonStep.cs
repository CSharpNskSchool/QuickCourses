﻿using System.Collections.Generic;

namespace QuickCourses.Models.Primitives
{
    public class LessonStep
    {
        public int Id { get; set; }
        public EducationalMaterial EducationalMaterial { get; set; }
        public List<Question> Questions { get; set; }
    }
}

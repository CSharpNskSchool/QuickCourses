using System.Collections.Generic;

namespace QuickCourses.Models.Progress
{
    public class LessonProgress
    {
        public int LessonId { get; set; }
        public List<LessonStepProgress> LessonStepProgress { get; set; }
        public bool Passed { get; set; }
    }    
}

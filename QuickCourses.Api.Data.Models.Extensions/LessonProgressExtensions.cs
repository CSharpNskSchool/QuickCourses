using System.Linq;
using QuickCourses.Api.Data.Models.Progress;
using QuickCourses.Api.Models.Progress;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class LessonProgressExtensions
    {
        public static LessonProgress ToApiModel(this LessonProgressData lessonProgressData)
        {
            var result = new LessonProgress
            {
                ProgressId = lessonProgressData.ProgressId,
                CourseId = lessonProgressData.CourseId,
                LessonId = lessonProgressData.LessonId,
                Passed = lessonProgressData.Passed,
                StepProgresses = lessonProgressData.StepProgresses
                                                        .Select(progress => progress.ToApiModel())
                                                        .ToList()
            };

            return result;
        }
    }
}

using System.Linq;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class LessonExtensions
    {
        public static Lesson ToApiModel(this Primitives.LessonData lessonData)
        {
            var result = new Lesson
            {
                CourseId = lessonData.CourseId,
                Description = lessonData.DescriptionData.ToApiModel(),
                Id = lessonData.Id,
                Steps = lessonData.Steps.Select(step => step.ToApiModel()).ToList()
            };

            return result;
        }
    }
}

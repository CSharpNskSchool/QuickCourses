using System.Linq;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Models.Extensions
{
    public static class LessonExtensions
    {
        public static LessonData ToDataModel(this Lesson lesson)
        {
            var result = new LessonData
            {
                CourseId = lesson.CourseId,
                Id = lesson.Id,
                DescriptionData = lesson.Description.ToDataModel(),
                Steps = lesson.Steps
                    .Select(step => step.ToDataModel())
                    .ToList()
            };

            return result;
        }
    }
}

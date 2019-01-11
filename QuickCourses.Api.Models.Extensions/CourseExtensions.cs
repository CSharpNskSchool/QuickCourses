using System.Linq;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Models.Extensions
{
    public static class CourseExtensions
    {
        public static CourseData ToDataModel(this Course course)
        {
            var result = new CourseData
            {
                Id = course.Id,
                DescriptionData = course.Description.ToDataModel(),
                Lessons = course.Lessons
                    .Select(lesson => lesson.ToDataModel())
                    .ToList(),
                Category = course.Category,
                Version = course.Version
            };

            return result;
        }
    }
}

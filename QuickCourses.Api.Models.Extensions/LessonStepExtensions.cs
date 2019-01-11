using System.Linq;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Models.Extensions
{
    public static class LessonStepExtensions
    {
        public static LessonStepData ToDataModel(this LessonStep step)
        {
            var result = new LessonStepData
            {
                CourseId = step.CourseId,
                LessonId = step.LessonId,
                Id = step.Id,
                EducationalMaterialData = step.EducationalMaterial.ToDataModel(),
                Questions = step.Questions
                    .Select(question => question.ToDataModel())
                    .ToList()
            };

            return result;
        }
    }
}

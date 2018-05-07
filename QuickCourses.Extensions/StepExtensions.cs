using System.Linq;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class StepExtensions
    {
        public static LessonStep ToApiModel(this Primitives.LessonStepData lessonStepData)
        {
            var result = new LessonStep
            {
                CourseId = lessonStepData.CourseId,
                LessonId = lessonStepData.LessonId,
                Id = lessonStepData.Id,
                EducationalMaterial = new EducationalMaterial
                {
                    Article = lessonStepData.EducationalMaterialData.Article,
                    Description = lessonStepData.EducationalMaterialData.DescriptionData.ToApiModel()
                },
                Questions = lessonStepData.Questions.Select(question => question.ToApiModel()).ToList()
            };

            return result;
        }
    }
}

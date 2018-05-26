using System.Linq;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class LessonStepDataExtensions
    {
        public static LessonStep ToApiModel(this LessonStepData lessonStepData)
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

        public static LessonStepData ToApiLessonStepData(this LessonStepData lessonStepData, QuestionData questionData)
        {
            questionData.CourseId = lessonStepData.CourseId;
            questionData.LessonId = lessonStepData.LessonId;
            questionData.StepId = lessonStepData.Id;
            questionData.Id = lessonStepData.Questions.Count;
            
            lessonStepData.Questions.Add(questionData);

            return lessonStepData;
        }
    }
}

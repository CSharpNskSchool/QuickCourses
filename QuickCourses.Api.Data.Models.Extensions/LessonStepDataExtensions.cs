using System;
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

        public static LessonStepData AddQuestionData(this LessonStepData lessonStepData, QuestionData questionData, out int questionId)
        {
            questionId = lessonStepData.Questions.Count;

            SetUpLinks(lessonStepData, questionData, questionId);

            lessonStepData.Questions.Add(questionData);

            return lessonStepData;
        }

        public static LessonStepData ReplaceQuetionData(this LessonStepData lessonStepData, int questionId, QuestionData questionData)
        {
            if (lessonStepData == null)
            {
                throw new ArgumentNullException(nameof(lessonStepData));
            }

            if (questionData == null)
            {
                throw new ArgumentNullException(nameof(questionData));
            }

            if (lessonStepData.Questions.Count <= questionId)
            {
                throw new ArgumentException($"Course doesn't contains lesson with id {questionId}");
            }

            SetUpLinks(lessonStepData, questionData, questionId);

            lessonStepData.Questions[questionId] = questionData;

            return lessonStepData;
        }

        public static LessonStepData RemoveLessonStepData(this LessonStepData lessonStepData, int stepId)
        {
            if (lessonStepData == null)
            {
                throw new ArgumentNullException(nameof(lessonStepData));
            }

            if (lessonStepData.Questions.Count <= stepId)
            {
                throw new ArgumentException($"Course doesn't contains lesson with id {stepId}");
            }

            lessonStepData.Questions.RemoveAt(stepId);

            for (var i = stepId; i < lessonStepData.Questions.Count; i++)
            {
                lessonStepData.Questions[i].Id = i;
            }

            return lessonStepData;
        }

        private static LessonStepData SetUpLinks(LessonStepData lessonStepData, QuestionData questionData, int questionId)
        {
            questionData.CourseId = lessonStepData.CourseId;
            questionData.LessonId = lessonStepData.LessonId;
            questionData.StepId = lessonStepData.Id;
            questionData.Id = questionId;

            return lessonStepData;
        }
    }
}

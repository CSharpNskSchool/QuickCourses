using System;
using System.Linq;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class LessonDataExtensions
    {
        public static Lesson ToApiModel(this LessonData lessonData)
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

        public static LessonData AddLessonStepData(this LessonData lessonData, LessonStepData lessonStepData, out int stepId)
        {
            stepId = lessonData.Steps.Count;

            SetUpLinks(lessonData, lessonStepData, stepId);

            lessonData.Steps.Add(lessonStepData);

            return lessonData;
        }

        public static LessonData ReplaceLessonStepData(this LessonData lessonData, int stepId, LessonStepData lessonStepData)
        {
            if (lessonData == null)
            {
                throw new ArgumentNullException(nameof(lessonData));
            }
            if (lessonStepData == null)
            {
                throw new ArgumentNullException(nameof(lessonStepData));
            }

            if (lessonData.Steps.Count <= stepId)
            {
                throw new ArgumentException($"Course doesn't contains lesson with id {stepId}");
            }

            SetUpLinks(lessonData, lessonStepData, stepId);

            lessonData.Steps[stepId] = lessonStepData;

            return lessonData;
        }

        public static LessonData RemoveLessonStepData(this LessonData lessonData, int stepId)
        {
            if (lessonData == null)
            {
                throw new ArgumentNullException(nameof(lessonData));
            }

            if (lessonData.Steps.Count <= stepId)
            {
                throw new ArgumentException($"Course doesn't contains lesson with id {stepId}");
            }

            lessonData.Steps.RemoveAt(stepId);

            for (var i = stepId; i < lessonData.Steps.Count; i++)
            {
                lessonData.Steps[i].Id = i;
            }

            return lessonData;
        }

        public static bool ContainsStep(this LessonData lessonData, int lessonId)
        {
            if (lessonData == null)
            {
                throw new ArgumentNullException(nameof(lessonData));
            }

            return lessonData.Steps.Count < lessonId;
        }
        
        private static LessonStepData SetUpLinks(LessonData lessonData, LessonStepData lessonStepData, int stepId)
        {
            lessonStepData.CourseId = lessonData.CourseId;
            lessonStepData.LessonId = lessonData.Id;
            lessonStepData.Id = stepId;

            return lessonStepData;
        }
    }
}

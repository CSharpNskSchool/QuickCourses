using System;
using System.Collections.Generic;
using System.Linq;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Data.Models.Progress;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class QuestionExtension
    {
        public static QuestionStateData GetQuestionState(this QuestionData questionData)
        {
            if (questionData == null)
            {
                throw new ArgumentNullException(nameof(questionData));
            }
            
            var result = new QuestionStateData
            {
                QuestionId = questionData.Id,
                CourseId = questionData.CourseId,
                LessonId = questionData.LessonId,
                StepId = questionData.StepId,
                SelectedAnswers = new List<int>(),
                CorrectlySelectedAnswers = new List<int>()
            };

            return result;
        }

        public static Question ToApiModel(this QuestionData questionData)
        {
            var result = new Question
            {
                CourseId = questionData.CourseId,
                LessonId = questionData.LessonId,
                StepId = questionData.StepId,
                Id = questionData.Id,
                Text = questionData.Text,
                TotalAttemptsCount = questionData.TotalAttemptsCount,
                AnswerVariants = questionData.AnswerVariants
                    .Select(variant => new AnswerVariant {Id = variant.Id, Text = variant.Text})
                    .ToList(),
                CorrectAnswers = new List<int>(questionData.CorrectAnswers)
            };

            return result;
        }
    }
}
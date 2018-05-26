using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Models.Extensions
{
    public static class QuistionExtensions
    {
        public static QuestionData ToDataModel(this Question question)
        {
            var result = new QuestionData
            {
                CourseId = question.CourseId,
                LessonId = question.LessonId,
                StepId = question.StepId,
                Id = question.Id,
                Text = question.Text,
                AnswerVariants = question.AnswerVariants
                    .Select(answerVariant => answerVariant.ToDataModel())
                    .ToList(),
                CorrectAnswers = question.CorrectAnswers.ToList(),
                TotalAttemptsCount = question.TotalAttemptsCount
            };

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Data.Models.Progress;
using QuickCourses.Api.Models.Progress;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class QuestionStateExtensions
    {
        public static QuestionStateData GetUpdated(this QuestionStateData stateData, QuestionData questionData, List<int> selected)
        {
            if (stateData == null)
            {
                throw new ArgumentNullException(nameof(stateData));
            }

            if (questionData == null)
            {
                throw new ArgumentNullException(nameof(questionData));
            }

            if (selected == null)
            {
                throw new ArgumentNullException(nameof(selected));
            }

            var result = new QuestionStateData
            {
                ProgressId = stateData.ProgressId,
                CourseId = stateData.CourseId,
                SelectedAnswers = new List<int>(selected),
                CorrectlySelectedAnswers = selected
                    .Where(x => questionData.CorrectAnswers.Contains(x))
                    .ToList(),
                CurrentAttemptsCount = stateData.CurrentAttemptsCount + 1
            };

            result.Passed = result.SelectedAnswers.Count == result.CorrectlySelectedAnswers.Count;

            return result;
        }

        public static QuestionState ToApiModel(this QuestionStateData questionStateData)
        {
            var result = new QuestionState
            {
                ProgressId = questionStateData.ProgressId,
                CourseId = questionStateData.CourseId,
                LessonId = questionStateData.LessonId,
                StepId = questionStateData.StepId,
                QuestionId = questionStateData.QuestionId,
                CurrentAttemptsCount = questionStateData.CurrentAttemptsCount,
                Passed = questionStateData.Passed,
                CorrectlySelectedAnswers = new List<int>(questionStateData.CorrectlySelectedAnswers),
                SelectedAnswers = new List<int>(questionStateData.SelectedAnswers)
            };

            return result;
        }
    }
}
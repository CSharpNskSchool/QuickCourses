using System.Collections.Generic;
using QuickCourses.Models.Interaction;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Progress;

namespace QuickCourses.Extensions
{
    public static class QuestionExtensions
    {
        public static QuestionState GetQuestionState(this Question question, Answer answer, int currentAttemptsCount)
        {
            var result = new QuestionState
            {
                CourseId = question.CourseId,
                LessonId = question.LessonId,
                StepId = question.StepId,
                SelectedAnswers = new List<int>(answer.SelectedAnswers),
                CorrectlySelectedAnswers = new List<int>(),
                CurrentAttemptsCount = currentAttemptsCount
            };

            foreach (var selectedAnswer in result.SelectedAnswers)
            {
                if (question.CorrectAnswers.Contains(selectedAnswer))
                {
                    result.CorrectlySelectedAnswers.Add(selectedAnswer);
                }
            }

            if (result.CorrectlySelectedAnswers.Count == question.CorrectAnswers.Count)
            {
                result.Passed = true;
            }

            return result;
        }
    }
}
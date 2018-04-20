﻿using System.Collections.Generic;
using QuickCourses.Models.Interaction;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Extentions
{
    public static class QuestionExtentions
    {
        public static QuestionState GetQuestionState(this Question question, Answer answer, QuestionState questionState)
        {
            var result = new QuestionState
            {
                CourseId = question.CourseId,
                LessonId = question.LessonId,
                StepId = question.LessondStepId,
                SelectedAnswers = new List<int>(answer.SelectedAnswers),
                CorrectlySelectedAnswers = new List<int>(),
                CurrentAttemptsCount = questionState.CurrentAttemptsCount + 1
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
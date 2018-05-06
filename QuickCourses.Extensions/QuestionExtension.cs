using System;
using System.Collections.Generic;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Progress;

namespace QuickCourses.Extensions
{
    public static class QuestionExtension
    {
        public static QuestionState GetQuestionState(this Question question)
        {
            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }
            
            var result = new QuestionState
            {
                CourseId = question.CourseId,
                LessonId = question.LessonId,
                StepId = question.StepId,
                SelectedAnswers = new List<int>(),
                CorrectlySelectedAnswers = new List<int>()
            };

            return result;
        }
    }
}
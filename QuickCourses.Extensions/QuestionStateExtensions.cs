using System;
using System.Collections.Generic;
using System.Linq;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Progress;

namespace QuickCourses.Extensions
{
    public static class QuestionStateExtensions
    {
        public static QuestionState Update(this QuestionState state, Question question, List<int> selected)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (question == null)
            {
                throw new ArgumentNullException(nameof(question));
            }

            if (selected == null)
            {
                throw new ArgumentNullException(nameof(selected));
            }
            
            state.SelectedAnswers = new List<int>(selected);
            state.CorrectlySelectedAnswers = selected.Where(x => question.CorrectAnswers.Contains(x)).ToList();
            state.Passed = state.SelectedAnswers.Count == state.CorrectlySelectedAnswers.Count;
            state.CurrentAttemptsCount++;
            return state;
        }
    }
}
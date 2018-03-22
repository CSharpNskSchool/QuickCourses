using System.Collections.Generic;
using QuickCourses.Model.Interaction;
using QuickCourses.Model.Primitives;
using QuickCourses.Model.Progress;

namespace QuickCourses.Api.Extentions
{
    public static class QuestionExtentions
    {
        public static QuestionState GetQuestionState(this Question question, Answer answer)
        {
            var result = new QuestionState();
            result.SelectedAnswers = new List<int>(answer.SelectedAnswers);
            
            foreach (var selectedAnswer in result.SelectedAnswers)
            {
                if (question.CorrectAnswers.Contains(selectedAnswer))
                {
                    result.CorrectlySelectedAnswers.Add(selectedAnswer);
                }
            }

            return result;
        }
    }
}
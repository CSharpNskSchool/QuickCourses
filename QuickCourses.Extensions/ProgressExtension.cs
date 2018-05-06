using System;
using QuickCourses.Models.Progress;

namespace QuickCourses.Extensions
{
    public static class ProgressExtension
    {
        public static Progress Update(
            this Progress progress,
            int lessonId,
            int stepId,
            int questionId,
            QuestionState questionState)
        {
            if (progress == null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            if (questionState == null)
            {
                throw new ArgumentNullException(nameof(questionState));
            }

            var lessonProgress = progress.LessonProgresses[lessonId];
            var stepProgress = lessonProgress.StepProgresses[stepId];
            var questionStates = stepProgress.QuestionStates;

            progress.Statistics.PassedQuestionsCount += GetDelta(questionStates[questionId], questionState);
            
            questionStates[questionId] = questionState;

            stepProgress.Passed = stepProgress.QuestionStates.TrueForAll(x => x.Passed);
            lessonProgress.Passed = lessonProgress.StepProgresses.TrueForAll(x => x.Passed);
            progress.Passed = progress.LessonProgresses.TrueForAll(x => x.Passed);

            return progress;
        }

        //Этот метод точно такой же как и SetUpLinks для Course только с другими названиями переменных
        //можно что-то продумать, наверное
        public static Progress SetUpLinks(this Progress progress)
        {
            if (progress == null)
            {
                throw new ArgumentNullException(nameof(progress));
            }

            foreach (var lessonProgress in progress.LessonProgresses)
            {
                lessonProgress.CourseId = progress.CourceId;

                foreach (var stepProgress in lessonProgress.StepProgresses)
                {
                    stepProgress.CourseId = progress.CourceId;
                    stepProgress.LessonId = lessonProgress.LessonId;

                    foreach (var questionState in stepProgress.QuestionStates)
                    {
                        questionState.ProgressId = progress.Id;
                        questionState.CourseId = progress.CourceId;
                        questionState.LessonId = lessonProgress.LessonId;
                        questionState.StepId = stepProgress.StepId;
                    }
                }
            }

            return progress;
        }

        private static int GetDelta(QuestionState curState, QuestionState newState)
        {
            return Convert.ToInt32(newState.Passed) - Convert.ToInt32(curState.Passed);
        }
    }
}
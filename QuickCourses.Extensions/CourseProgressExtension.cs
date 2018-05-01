using System;
using QuickCourses.Models.Progress;

namespace QuickCourses.Extensions
{
    public static class CourseProgressExtension
    {
        public static CourseProgress Update(
            this CourseProgress courseProgress,
            int lessonId,
            int stepId,
            int questionId,
            QuestionState questionState)
        {
            if (courseProgress == null)
            {
                throw new ArgumentNullException(nameof(courseProgress));
            }

            if (questionState == null)
            {
                throw new ArgumentNullException(nameof(questionState));
            }

            var lessonProgress = courseProgress.LessonProgresses[lessonId];
            var stepProgress = lessonProgress.StepProgresses[stepId];
            var questionStates = stepProgress.QuestionStates;

            courseProgress.Statistics.PassedQuestionsCount += GetDelta(questionStates[questionId], questionState);
            
            questionStates[questionId] = questionState;

            stepProgress.Passed = stepProgress.QuestionStates.TrueForAll(x => x.Passed);
            lessonProgress.Passed = lessonProgress.StepProgresses.TrueForAll(x => x.Passed);
            courseProgress.Passed = courseProgress.LessonProgresses.TrueForAll(x => x.Passed);

            return courseProgress;
        }

        //Этот метод точно такой же как и SetUpLinks для Course только с другими названиями переменных
        //можно что-то продумать, наверное
        public static CourseProgress SetUpLinks(this CourseProgress courseProgress)
        {
            if (courseProgress == null)
            {
                throw new ArgumentNullException(nameof(courseProgress));
            }

            foreach (var lessonProgress in courseProgress.LessonProgresses)
            {
                lessonProgress.CourseId = courseProgress.CourceId;

                foreach (var stepProgress in lessonProgress.StepProgresses)
                {
                    stepProgress.CourseId = courseProgress.CourceId;
                    stepProgress.LessonId = lessonProgress.LessonId;

                    foreach (var questionState in stepProgress.QuestionStates)
                    {
                        questionState.CourseId = courseProgress.CourceId;
                        questionState.LessonId = lessonProgress.LessonId;
                        questionState.StepId = stepProgress.StepId;
                    }
                }
            }

            return courseProgress;
        }

        private static int GetDelta(QuestionState curState, QuestionState newState)
        {
            return Convert.ToInt32(newState.Passed) - Convert.ToInt32(curState.Passed);
        }
    }
}
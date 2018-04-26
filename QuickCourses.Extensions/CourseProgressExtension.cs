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
            var lessonProgress = courseProgress.LessonProgresses[lessonId];
            var stepProgress = lessonProgress.LessonStepProgress[stepId];
            var questionStates = stepProgress.QuestionStates;

            courseProgress.Statistics.PassedQuestionsCount += GetDelta(questionStates[questionId], questionState);
            
            questionStates[questionId] = questionState;
            
            if (stepProgress.QuestionStates.TrueForAll(x => x.Passed))
            {
                stepProgress.Passed = true;
            }

            if (lessonProgress.LessonStepProgress.TrueForAll(x => x.Passed))
            {
                lessonProgress.Passed = true;
            }

            if (courseProgress.LessonProgresses.TrueForAll(x => x.Passed))
            {
                courseProgress.Passed = true;
            }

            return courseProgress;
        }

        //Этот метод точно такой же как и SetUpLinks для Course только с другими названиями переменных
        //можно что-то продумать, наверное
        public static CourseProgress SetUpLinks(this CourseProgress courseProgress)
        {
            foreach (var lessonProgress in courseProgress.LessonProgresses)
            {
                lessonProgress.CourseId = courseProgress.CourceId;

                foreach (var stepProgress in lessonProgress.LessonStepProgress)
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
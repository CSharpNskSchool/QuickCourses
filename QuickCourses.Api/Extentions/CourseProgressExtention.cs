﻿using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Extentions
{
    public static class CourseProgressExtention
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

        private static int GetDelta(QuestionState curState, QuestionState newState)
        {
            return (newState.Passed ? 1 : 0) - (curState.Passed ? 1 : 0);
        }
    }
}
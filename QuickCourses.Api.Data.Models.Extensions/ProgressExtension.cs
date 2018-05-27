using System;
using System.Linq;
using QuickCourses.Api.Data.Models.Progress;
using QuickCourses.Api.Models.Progress;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class ProgressExtension
    {
        public static CourseProgressData Update(
            this CourseProgressData progressData,
            int lessonId,
            int stepId,
            int questionId,
            QuestionStateData questionStateData)
        {
            if (progressData == null)
            {
                throw new ArgumentNullException(nameof(progressData));
            }

            if (questionStateData == null)
            {
                throw new ArgumentNullException(nameof(questionStateData));
            }

            var lessonProgress = progressData.LessonProgresses[lessonId];
            var stepProgress = lessonProgress.StepProgresses[stepId];
            var questionStates = stepProgress.QuestionStates;

            progressData.StatisticsData.PassedQuestionsCount += GetDelta(questionStates[questionId], questionStateData);
            progressData.StatisticsData.Passed = progressData.StatisticsData.PassedQuestionsCount ==
                                                 progressData.StatisticsData.TotalQuestionsCount; 

            questionStates[questionId] = questionStateData;

            stepProgress.Passed = stepProgress.QuestionStates.TrueForAll(x => x.Passed);
            lessonProgress.Passed = lessonProgress.StepProgresses.TrueForAll(x => x.Passed);
            progressData.Passed = progressData.LessonProgresses.TrueForAll(x => x.Passed);

            return progressData;
        }
        
        public static CourseProgressData SetUpLinks(this CourseProgressData courseProgressData)
        {
            if (courseProgressData == null)
            {
                throw new ArgumentNullException(nameof(courseProgressData));
            }

            foreach (var lessonProgress in courseProgressData.LessonProgresses)
            {
                lessonProgress.ProgressId = courseProgressData.Id;
                lessonProgress.CourseId = courseProgressData.CourceId;

                foreach (var stepProgress in lessonProgress.StepProgresses)
                {
                    stepProgress.ProgressId = courseProgressData.Id;
                    stepProgress.CourseId = courseProgressData.CourceId;
                    stepProgress.LessonId = lessonProgress.LessonId;

                    foreach (var questionState in stepProgress.QuestionStates)
                    {
                        questionState.ProgressId = courseProgressData.Id;
                        questionState.CourseId = courseProgressData.CourceId;
                        questionState.LessonId = lessonProgress.LessonId;
                        questionState.StepId = stepProgress.Id;
                    }
                }
            }

            return courseProgressData;
        }

        public static CourseProgress ToApiModel(this CourseProgressData progressData)
        {
            var result = new CourseProgress
            {
                CourceId = progressData.CourceId,
                Id = progressData.Id,
                LessonProgresses =
                    progressData.LessonProgresses.Select(lessonProgress => lessonProgress.ToApiModel()).ToList(),
                Passed = progressData.Passed,
                Statistics = new Statistics
                {
                    Passed = progressData.StatisticsData.Passed,
                    PassedQuestionsCount = progressData.StatisticsData.PassedQuestionsCount,
                    TotalQuestionsCount = progressData.StatisticsData.TotalQuestionsCount
                },
                UserId = progressData.UserId
            };

            return result;
        }

        private static int GetDelta(QuestionStateData curStateData, QuestionStateData newStateData)
        {
            return Convert.ToInt32(newStateData.Passed) - Convert.ToInt32(curStateData.Passed);
        }
    }
}
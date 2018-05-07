using System;
using System.Collections.Generic;
using System.Linq;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Data.Models.Progress;
using QuickCourses.Api.Extensions;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class CourseExtensions
    {
        public static CourseProgressData CreateProgress(this CourseData courseData, string userId)
        {
            if (courseData == null)
            {
                throw new ArgumentNullException(nameof(courseData));
            }

            var result = new CourseProgressData
            {
                LessonProgresses = new List<LessonProgressData>(),
                CourceId = courseData.Id,
                UserId = userId,
                StatisticsData = new StatisticsData {TotalQuestionsCount = QuestionsCount(courseData)}
            };

            foreach (var lesson in courseData.Lessons)
            {
                AddLessonProgress(result, lesson);
            }

            return result;
        }

        public static QuestionData GetQuestion(this CourseData courseData, int lessonId, int stepId, int questionId)
        {
            if (courseData == null)
            {
                throw new ArgumentNullException(nameof(courseData));
            }

            if (!courseData.Lessons.TryGetValue(lessonId, out var lesson))
            {
                return null;
            }

            if (!lesson.Steps.TryGetValue(stepId, out var step))
            {
                return null;
            }

            step.Questions.TryGetValue(questionId, out var result);

            return result;
        }

        public static CourseData SetUpLinks(this CourseData courseData)
        {
            if (courseData == null)
            {
                throw new ArgumentNullException(nameof(courseData));
            }

            foreach (var lesson in courseData.Lessons)
            {
                lesson.CourseId = courseData.Id;

                foreach (var step in lesson.Steps)
                {
                    step.CourseId = courseData.Id;
                    step.LessonId = lesson.Id;

                    foreach (var question in step.Questions)
                    {
                        question.CourseId = courseData.Id;
                        question.LessonId = lesson.Id;
                        question.StepId = step.Id;
                    }
                }
            }

            return courseData;
        }

        public static Course ToApiModel(this CourseData courseData)
        {
            var result = new Course 
            {
                Description = courseData.DescriptionData.ToApiModel(),
                Id = courseData.Id,
                Lessons = courseData.Lessons.Select(x => x.ToApiModel()).ToList()
            };

            return result;
        }

        private static int QuestionsCount(CourseData courseData)
        {
            if (courseData == null)
            {
                throw new ArgumentNullException(nameof(courseData));
            }

            return courseData.Lessons.Sum(lesson => lesson.Steps.Sum(step => step.Questions.Count));
        }

        private static void AddLessonProgress(CourseProgressData courseProgressData, LessonData lessonData)
        {
            var lessonProgress = new LessonProgressData
            {
                LessonId = lessonData.Id,
                StepProgresses = new List<StepProgressData>()
            };

            foreach (var step in lessonData.Steps)
            {
                AddStepProgress(lessonProgress, step);
            }

            courseProgressData.LessonProgresses.Add(lessonProgress);
        }

        private static void AddStepProgress(LessonProgressData lessonProgressData, LessonStepData stepData)
        {
            var stepProgress = new StepProgressData
            {
                Id = stepData.Id,
                QuestionStates = new List<QuestionStateData>()
            };

            foreach (var question in stepData.Questions)
            {
                var questionState = question.GetQuestionState();
                stepProgress.QuestionStates.Add(questionState);
            }

            lessonProgressData.StepProgresses.Add(stepProgress);
        }
    }
}
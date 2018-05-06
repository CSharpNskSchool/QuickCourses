using System;
using System.Collections.Generic;
using System.Linq;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Progress;

namespace QuickCourses.Extensions
{
    public static class CourseExtensions
    {
        public static CourseProgress CreateProgress(this Course course, string userId)
        {
            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }

            var result = new CourseProgress
            {
                LessonProgresses = new List<LessonProgress>(),
                CourceId = course.Id,
                UserId = userId,
                Statistics = new Statistics {TotalQuestionsCount = QuestionsCount(course)}
            };

            foreach (var lesson in course.Lessons)
            {
                AddLessonProgress(result, lesson);
            }

            return result;
        }

        public static Question GetQuestion(this Course course, int lessonId, int stepId, int questionId)
        {
            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }

            if (!course.Lessons.TryGetValue(lessonId, out var lesson))
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

        public static Course SetUpLinks(this Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }

            foreach (var lesson in course.Lessons)
            {
                lesson.CourseId = course.Id;

                foreach (var step in lesson.Steps)
                {
                    step.CourseId = course.Id;
                    step.LessonId = lesson.Id;

                    foreach (var question in step.Questions)
                    {
                        question.CourseId = course.Id;
                        question.LessonId = lesson.Id;
                        question.StepId = step.Id;
                    }
                }
            }

            return course;
        }

        private static int QuestionsCount(Course course)
        {
            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }

            return course.Lessons.Sum(lesson => lesson.Steps.Sum(step => step.Questions.Count));
        }

        private static void AddLessonProgress(CourseProgress courseProgress, Lesson lesson)
        {
            var lessonProgress = new LessonProgress
            {
                LessonId = lesson.Id,
                StepProgresses = new List<StepProgress>()
            };

            foreach (var step in lesson.Steps)
            {
                AddStepProgress(lessonProgress, step);
            }

            courseProgress.LessonProgresses.Add(lessonProgress);
        }

        private static void AddStepProgress(LessonProgress lessonProgress, LessonStep step)
        {
            var stepProgress = new StepProgress
            {
                StepId = step.Id,
                QuestionStates = new List<QuestionState>()
            };

            foreach (var question in step.Questions)
            {
                var questionState = question.GetQuestionState();
                stepProgress.QuestionStates.Add(questionState);
            }

            lessonProgress.StepProgresses.Add(stepProgress);
        }
    }
}
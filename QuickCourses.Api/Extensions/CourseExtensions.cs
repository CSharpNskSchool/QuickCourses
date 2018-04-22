using System.Collections.Generic;
using System.Linq;
using QuickCourses.Models.Primitives;
using QuickCourses.Models.Progress;

namespace QuickCourses.Api.Extensions
{
    public static class CourseExtensions
    {
        public static CourseProgress CreateProgress(this Course course, string userId)
        {
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

        private static int QuestionsCount(Course course)
        {
            return course.Lessons.Sum(lesson => lesson.Steps.Sum(step => step.Questions.Count));
        }

        private static void AddLessonProgress(CourseProgress courseProgress, Lesson lesson)
        {
            var lessonProgress = new LessonProgress
            {
                LessonId = lesson.Id,
                LessonStepProgress = new List<LessonStepProgress>()
            };

            foreach (var step in lesson.Steps)
            {
                AddStepProgress(lessonProgress, step);
            }

            courseProgress.LessonProgresses.Add(lessonProgress);
        }

        private static void AddStepProgress(LessonProgress lessonProgress, LessonStep step)
        {
            var stepProgress = new LessonStepProgress
            {
                StepId = step.Id,
                QuestionStates = new List<QuestionState>()
            };

            for (var i = 0; i < step.Questions.Count; i++)
            {
                var questionState = new QuestionState
                {
                    CorrectlySelectedAnswers = new List<int>(),
                    SelectedAnswers = new List<int>()
                };

                stepProgress.QuestionStates.Add(questionState);
            }

            lessonProgress.LessonStepProgress.Add(stepProgress);
        }

        public static Question GetQuestion(this Course course, int lessonId, int stepId, int questionId)
        {
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
    }
}
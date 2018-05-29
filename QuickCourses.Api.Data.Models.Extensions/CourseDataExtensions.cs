using System;
using System.Collections.Generic;
using System.Linq;
using QuickCourses.Api.Data.Models.Primitives;
using QuickCourses.Api.Data.Models.Progress;
using QuickCourses.Api.Extensions;
using QuickCourses.Api.Models.Primitives;

namespace QuickCourses.Api.Data.Models.Extensions
{
    public static class CourseDataExtensions
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
            if (courseData == null)
            {
                throw new ArgumentNullException(nameof(courseData));
            }

            var result = new Course 
            {
                Description = courseData.DescriptionData.ToApiModel(),
                Id = courseData.Id,
                Category = courseData.Category,
                Version = courseData.Version,
                Lessons = courseData.Lessons.Select(x => x.ToApiModel()).ToList()
            };

            return result;
        }

        public static bool ContainsLesson(this CourseData courseData, int lessonId)
        {
            if (courseData == null)
            {
                throw new ArgumentNullException(nameof(courseData));
            }

            return courseData.Lessons.Count < lessonId;
        }

        public static CourseData AddLesson(this CourseData courseData, LessonData lessonData, out int lessonId)
        {
            if (courseData == null)
            {
                throw new ArgumentNullException(nameof(courseData));
            }

            if (lessonData == null)
            {
                throw new ArgumentNullException(nameof(lessonData));
            }

            lessonData.CourseId = courseData.Id;
            lessonId = courseData.Lessons.Count;;
            lessonData.Id = lessonId;

            courseData.Lessons.Add(lessonData);

            return courseData;
        }

        public static CourseData ReplaceLesson(this CourseData courseData, int lessonId, LessonData lesson)
        {
            if (courseData == null)
            {
                throw new ArgumentNullException(nameof(courseData));
            }

            if (lesson == null)
            {
                throw new ArgumentNullException(nameof(lesson));
            }

            if (courseData.Lessons.Count <= lessonId)
            {
                throw new ArgumentException($"Course doesn't contains lesson with id {lessonId}");
            }

            lesson.CourseId = courseData.Id;
            lesson.Id = lessonId;

            courseData.Lessons[lessonId] = lesson;

            return courseData;
        }

        public static CourseData RemoveLesson(this CourseData courseData, int lessonId)
        {
            if (courseData == null)
            {
                throw new ArgumentNullException(nameof(courseData));
            }

            if (courseData.Lessons.Count <= lessonId)
            {
                throw new ArgumentException($"Course doesn't contains lesson with id {lessonId}");
            }

            courseData.Lessons.RemoveAt(lessonId);

            for (var i = lessonId; i < courseData.Lessons.Count; i++)
            {
                courseData.Lessons[i].Id = i;
            }

            return courseData;
        }

        public static bool IsAuthor(this CourseData courseData, string userId)
        {
            return courseData.AuthorId == userId;
        }

        public static CourseData NextVersion(this CourseData courseData)
        {
            courseData.Version++;
            return courseData;
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
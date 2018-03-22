using System.Collections.Generic;
using QuickCourses.Model.Primitives;
using QuickCourses.Model.Progress;

namespace QuickCourses.Api.Extentions
{
    public static class CourseExtentions
    {
        public static CourseProgress CreateProgress(this Course course, int userId)
        {
            var result = new CourseProgress();
            result.CourceId = course.Id;
            result.UserId = userId;

            foreach (var lesson in course.Lessons)
            {
                var lessongProgress = new LessonProgress {LessonId = lesson.Id};
                
                foreach (var step in lesson.Steps)
                {
                    var stepProgress = new LessonStepProgress {StepId = step.Id};

                    for(int i = 0; i < step.Questions.Count; i++)
                    {
                        var questionState = new QuestionState
                        {
                            CorrectlySelectedAnswers = new List<int>(),
                            SelectedAnswers = new List<int>()
                        };
                        
                        stepProgress.QuestionStates.Add(questionState);
                    }

                    lessongProgress.LessonStepProgress.Add(stepProgress);
                }
                
                result.LessonProgresses.Add(lessongProgress);
            }

            return result;
        }

        public static Question GetQuestion(this Course course, int lessonId, int stepId, int questionId)
        {
            if (course.Lessons.Count < lessonId)
            {
                return null;
            }

            var lesson = course.Lessons[lessonId];

            if (lesson.Steps.Count < stepId)
            {
                return null;
            }

            var step = lesson.Steps[stepId];

            if (step.Questions.Count < questionId)
            {
                return null;
            }

            return step.Questions[questionId];
        }
    }
}
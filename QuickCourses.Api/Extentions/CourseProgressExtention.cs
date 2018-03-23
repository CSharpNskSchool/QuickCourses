using QuickCourses.Model.Progress;

namespace QuickCourses.Api.Extentions
{
    public static class CourseProgressExtention
    {
        public static void Update(
            this CourseProgress courseProgress,
            int lessonId,
            int stepId,
            int questionId,
            QuestionState questionState)
        {
            var lessonProgress = courseProgress.LessonProgresses[lessonId];
            var stepProgress = lessonProgress.LessonStepProgress[stepId];

            stepProgress.QuestionStates[questionId] = questionState;

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
        }
    }
}
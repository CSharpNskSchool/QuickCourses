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
            courseProgress.LessonProgresses[lessonId].LessonStepProgress[stepId].QuestionStates[questionId] = questionState;
        }
    }
}
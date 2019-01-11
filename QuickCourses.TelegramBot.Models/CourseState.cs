namespace QuickCourses.TelegramBot.Models
{
    public class CourseState
    {
        public string ProgressId { get; set; }
        public string CourseId { get; set; }
        public int StepId { get; set; }
        public int LessonId { get; set; }
        public int QuestionId { get; set; }
    }
}

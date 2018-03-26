namespace QuickCourses.Models
{
    public class Error
    {
        public enum ErrorCode
        {
            BadArgument
        }

        public ErrorCode Code;
        public string Message;
    }
}
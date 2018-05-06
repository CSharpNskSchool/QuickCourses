namespace QuickCourses.Models.Errors
{
    public class Error
    {
        public enum ErrorCode
        {
            BadArgument,
            NotFound,
            InvalidOperation,
            Forbidden
        }

        public ErrorCode Code;
        public string Message;
    }
}
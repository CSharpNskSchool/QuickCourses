namespace QuickCourses.Models.Errors
{
    public class Error
    {
        public enum ErrorCode
        {
            BadArgument,
            NotFound,
            InvalidOperation
        }

        public ErrorCode Code;
        public string Message;
    }
}
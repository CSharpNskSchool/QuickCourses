namespace QuickCourses.Api.Models.Errors
{
    public class Error
    {
        public enum ErrorCode
        {
            BadArgument,
            NotFound,
            InvalidOperation,
            Forbidden,
            Internal
        }

        public ErrorCode Code;
        public string Message;
    }
}
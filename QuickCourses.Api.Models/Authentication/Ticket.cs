using System;

namespace QuickCourses.Api.Models.Authentication
{
    public class Ticket
    {
        public string Source { get; set; }
        public DateTime ValidUntil { get; set; }

        public override string ToString()
        {
            return $"Bearer {Source}";
        }
    }
}

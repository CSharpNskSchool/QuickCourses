using System;

namespace QuickCourses.Models.Authentication
{
    public class Ticket
    {
        public string Source { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}

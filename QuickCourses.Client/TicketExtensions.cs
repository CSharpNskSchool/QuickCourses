using QuickCourses.Models.Authentication;
using System;

namespace QuickCourses.Client
{
    public static class TicketExtensions
    {
        public static bool IsValid(this Ticket ticket)
        {
            return ticket.ValidUntil < DateTime.Now;
        }
    }
}

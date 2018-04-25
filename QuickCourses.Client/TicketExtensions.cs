using QuickCourses.Models.Authentication;
using System;

namespace QuickCourses.Client
{
    public static class TicketExtensions
    {
        public static bool IsValid(this Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            return ticket.ValidUntil < DateTime.Now;
        }
    }
}

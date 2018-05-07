using System;
using QuickCourses.Api.Models.Authentication;

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

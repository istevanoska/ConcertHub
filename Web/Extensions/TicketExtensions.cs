using Domain.Models;
using Web.Response;

namespace Web.Extensions;

public static class TicketExtensions
{
    public static TicketResponse ToResponse(this Ticket ticket)
    {
        var attendee = ticket.User == null
            ? string.Empty
            : $"{ticket.User.FirstName} {ticket.User.LastName}".Trim();

        return new TicketResponse(
            ticket.Id,
            ticket.UserId,
            attendee,
            ticket.ConcertId,
            ticket.Concert?.Title ?? string.Empty,
            ticket.TicketCategory?.Name ?? string.Empty,
            ticket.Status.ToString(),
            ticket.Price,
            ticket.SeatNumber);
    }

    public static TicketBasicResponse ToBasicResponse(this Ticket ticket)
    {
        return new TicketBasicResponse(
            ticket.Id,
            ticket.Status.ToString(),
            ticket.Price);
    }
}

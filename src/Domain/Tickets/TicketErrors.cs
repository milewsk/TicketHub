using Common;

namespace Domain.Tickets;

public static class TicketErrors
{
    public static Error TicketNotFound(Guid id) => Error.NotFound("Ticket.NotFound", $"Ticket with id: {id} not found");

    public static Error TicketAlreadyReserved() =>
        Error.Conflict("Ticket.AlreadyReserved", "Ticket already reserved.");
}
using Application.Abstractions.Messaging;

namespace Application.Tickets.Reserve;

public sealed record ReserveTicketCommand(Guid TicketId, Guid UserId) : ICommand;
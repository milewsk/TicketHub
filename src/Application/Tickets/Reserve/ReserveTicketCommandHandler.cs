using Application.Abstractions.Data;
using Application.Abstractions.Messaging;

using Common;

using Domain.Tickets;

using Microsoft.EntityFrameworkCore;

namespace Application.Tickets.Reserve;

internal sealed class ReserveTicketCommandHandler(IApplicationDbContext context)
    : ICommandHandler<ReserveTicketCommand>
{
    public async Task<Result<Guid>> HandleAsync(ReserveTicketCommand command,
        CancellationToken cancellationToken)
    {
        Ticket? ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == command.TicketId, cancellationToken);

        if (ticket == null)
        {
            return Result.Failure<Guid>(TicketErrors.TicketNotFound(id: command.TicketId));
        }

        if (ticket.IsReserved)
        {
            return Result.Failure<Guid>(TicketErrors.TicketAlreadyReserved());
        }

        ticket.Reserve();

        // domain event

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success<Guid>(ticket.Id);
    }
}
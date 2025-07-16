using FluentValidation;

namespace Application.Tickets.Reserve;

internal sealed class ReserveTicketCommandValidator : AbstractValidator<ReserveTicketCommand>
{
    public ReserveTicketCommandValidator()
    {
        RuleFor(reserveTicket => reserveTicket.TicketId).NotEmpty();
    }
}
using Common;

namespace Domain.Tickets;

public class Ticket : AuditableEntity
{
    public Guid EventID { get; private set; }
    public TicketType Type { get; private set; }
    public decimal Price { get; private set; }
    public bool IsReserved { get; private set; }
    public Seat? Seat { get; private set; }

    private Ticket()
    {
    }

    private Ticket(Guid eventID, TicketType type, decimal price, Seat? seat = null)
    {
        EventID = eventID;
        Type = type;
        Price = price;
        IsReserved = false;
        Seat = seat;
    }

    public void Reserve()
    {
        IsReserved = true;
    }
}
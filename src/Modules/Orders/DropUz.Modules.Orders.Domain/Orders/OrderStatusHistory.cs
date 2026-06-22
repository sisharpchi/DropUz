using DropUz.Common.Domain;

namespace DropUz.Modules.Orders.Domain.Orders;

public sealed class OrderStatusHistory : Entity
{
    private OrderStatusHistory()
    {
    }

    internal OrderStatusHistory(Guid id, Guid orderId, OrderStatus status, string? note, DateTime changedAtUtc)
        : base(id)
    {
        OrderId = orderId;
        Status = status;
        Note = note;
        ChangedAtUtc = changedAtUtc;
    }

    public Guid OrderId { get; private set; }

    public OrderStatus Status { get; private set; }

    public string? Note { get; private set; }

    public DateTime ChangedAtUtc { get; private set; }
}

using DropUz.Common.Domain;

namespace DropUz.Modules.Orders.Domain.Orders;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _items = [];
    private readonly List<OrderStatusHistory> _statusHistory = [];

    private Order()
    {
    }

    private Order(Guid id, Guid userId, Guid? sellerId, DateTime createdAtUtc)
        : base(id)
    {
        UserId = userId;
        SellerId = sellerId;
        Status = OrderStatus.PendingProductPayment;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public Guid UserId { get; private set; }

    public Guid? SellerId { get; private set; }

    public OrderStatus Status { get; private set; }

    public decimal ProductTotal { get; private set; }

    public decimal CargoTotal { get; private set; }

    public decimal Total { get; private set; }

    public decimal SellerProfitTotal { get; private set; }

    public DateTime? ProductPaidAtUtc { get; private set; }

    public DateTime? CargoPaidAtUtc { get; private set; }

    public DateTime? CargoPaymentDeadlineAt { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public IReadOnlyCollection<OrderStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    public static Order Create(
        Guid userId,
        Guid? sellerId,
        IReadOnlyCollection<OrderItemSnapshot> snapshots,
        DateTime createdAtUtc)
    {
        if (snapshots.Count == 0)
        {
            throw new ArgumentException("Order requires at least one item.", nameof(snapshots));
        }

        var order = new Order(Guid.NewGuid(), userId, sellerId, createdAtUtc);

        foreach (OrderItemSnapshot snapshot in snapshots)
        {
            order._items.Add(new OrderItem(Guid.NewGuid(), order.Id, snapshot));
        }

        order.RecalculateTotals();
        order.AddHistory(OrderStatus.PendingProductPayment, "Order created.", createdAtUtc);

        return order;
    }

    public void MarkProductPaid(DateTime nowUtc)
    {
        if (Status != OrderStatus.PendingProductPayment)
        {
            return;
        }

        ProductPaidAtUtc = nowUtc;
        ChangeStatus(OrderStatus.ProductPaid, "Product payment received.", nowUtc);
    }

    public void SetCargoPrice(decimal cargoPrice, int deadlineDays, DateTime nowUtc)
    {
        decimal perItemCargo = _items.Count == 0 ? cargoPrice : decimal.Round(cargoPrice / _items.Count, 2, MidpointRounding.AwayFromZero);
        foreach (OrderItem item in _items)
        {
            item.SetCargoPrice(perItemCargo);
        }

        CargoTotal = cargoPrice;
        Total = ProductTotal + CargoTotal;
        CargoPaymentDeadlineAt = nowUtc.AddDays(deadlineDays <= 0 ? 7 : deadlineDays);
        ChangeStatus(OrderStatus.PendingCargoPayment, "Cargo price added.", nowUtc);
    }

    public void MarkCargoPaid(DateTime nowUtc)
    {
        if (Status != OrderStatus.PendingCargoPayment)
        {
            return;
        }

        CargoPaidAtUtc = nowUtc;
        ChangeStatus(OrderStatus.CargoPaid, "Cargo payment received.", nowUtc);
    }

    public void UpdateStatus(OrderStatus status, string? note, DateTime nowUtc)
    {
        ChangeStatus(status, note, nowUtc);
    }

    public void ExpireCargoPayment(DateTime nowUtc)
    {
        if (Status == OrderStatus.PendingCargoPayment &&
            CargoPaymentDeadlineAt.HasValue &&
            CargoPaymentDeadlineAt.Value < nowUtc)
        {
            ChangeStatus(OrderStatus.CargoPaymentExpired, "Cargo payment deadline expired.", nowUtc);
        }
    }

    private void ChangeStatus(OrderStatus status, string? note, DateTime nowUtc)
    {
        Status = status;
        UpdatedAtUtc = nowUtc;
        AddHistory(status, note, nowUtc);
    }

    private void AddHistory(OrderStatus status, string? note, DateTime changedAtUtc)
    {
        _statusHistory.Add(new OrderStatusHistory(Guid.NewGuid(), Id, status, note, changedAtUtc));
    }

    private void RecalculateTotals()
    {
        ProductTotal = _items.Sum(item => item.ProductLineTotal);
        CargoTotal = _items.Sum(item => item.CargoPrice);
        Total = ProductTotal + CargoTotal;
        SellerProfitTotal = _items.Sum(item => item.SellerProfitTotal);
    }
}

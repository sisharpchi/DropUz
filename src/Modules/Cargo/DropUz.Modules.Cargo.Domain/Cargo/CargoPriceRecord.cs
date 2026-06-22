using DropUz.Common.Domain;

namespace DropUz.Modules.Cargo.Domain.Cargo;

public sealed class CargoPriceRecord : Entity
{
    private CargoPriceRecord()
    {
    }

    private CargoPriceRecord(Guid id, Guid orderId, decimal amount, DateTime deadlineAtUtc, DateTime createdAtUtc)
        : base(id)
    {
        OrderId = orderId;
        Amount = amount;
        DeadlineAtUtc = deadlineAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid OrderId { get; private set; }

    public decimal Amount { get; private set; }

    public DateTime DeadlineAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public static CargoPriceRecord Create(Guid orderId, decimal amount, DateTime deadlineAtUtc, DateTime createdAtUtc)
    {
        return new CargoPriceRecord(Guid.NewGuid(), orderId, amount, deadlineAtUtc, createdAtUtc);
    }
}

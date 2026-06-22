using DropUz.Common.Domain;

namespace DropUz.Modules.Sellers.Domain.Sellers;

public sealed class SellerBalanceTransaction : Entity
{
    private SellerBalanceTransaction()
    {
    }

    internal SellerBalanceTransaction(
        Guid id,
        Guid sellerId,
        Guid? orderId,
        SellerBalanceTransactionType type,
        decimal amount,
        string? note,
        DateTime createdAtUtc)
        : base(id)
    {
        SellerId = sellerId;
        OrderId = orderId;
        Type = type;
        Amount = amount;
        Note = note;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid SellerId { get; private set; }

    public Guid? OrderId { get; private set; }

    public SellerBalanceTransactionType Type { get; private set; }

    public decimal Amount { get; private set; }

    public string? Note { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }
}

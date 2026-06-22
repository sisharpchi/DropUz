using DropUz.Common.Domain;

namespace DropUz.Modules.Cart.Domain.Carts;

public sealed class CartItem : Entity
{
    private CartItem()
    {
    }

    internal CartItem(Guid id, Guid cartId, Guid productId, int quantity, DateTime createdAtUtc)
        : base(id)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public Guid CartId { get; private set; }

    public Guid ProductId { get; private set; }

    public int Quantity { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public void SetQuantity(int quantity, DateTime nowUtc)
    {
        Quantity = Math.Max(1, quantity);
        UpdatedAtUtc = nowUtc;
    }
}

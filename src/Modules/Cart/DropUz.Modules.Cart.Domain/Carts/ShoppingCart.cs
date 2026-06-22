using DropUz.Common.Domain;

namespace DropUz.Modules.Cart.Domain.Carts;

public sealed class ShoppingCart : Entity
{
    private readonly List<CartItem> _items = [];

    private ShoppingCart()
    {
    }

    private ShoppingCart(Guid id, Guid userId, Guid? sellerId, DateTime createdAtUtc)
        : base(id)
    {
        UserId = userId;
        SellerId = sellerId;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public Guid UserId { get; private set; }

    public Guid? SellerId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    public static ShoppingCart Create(Guid userId, Guid? sellerId, DateTime createdAtUtc)
    {
        return new ShoppingCart(Guid.NewGuid(), userId, sellerId, createdAtUtc);
    }

    public CartItem AddOrUpdateItem(Guid productId, int quantity, DateTime nowUtc)
    {
        CartItem? item = _items.FirstOrDefault(x => x.ProductId == productId);
        if (item is null)
        {
            item = new CartItem(Guid.NewGuid(), Id, productId, Math.Max(1, quantity), nowUtc);
            _items.Add(item);
        }
        else
        {
            item.SetQuantity(item.Quantity + Math.Max(1, quantity), nowUtc);
        }

        UpdatedAtUtc = nowUtc;

        return item;
    }

    public bool UpdateItem(Guid cartItemId, int quantity, DateTime nowUtc)
    {
        CartItem? item = _items.FirstOrDefault(x => x.Id == cartItemId);
        if (item is null)
        {
            return false;
        }

        item.SetQuantity(quantity, nowUtc);
        UpdatedAtUtc = nowUtc;

        return true;
    }

    public bool RemoveItem(Guid cartItemId, DateTime nowUtc)
    {
        CartItem? item = _items.FirstOrDefault(x => x.Id == cartItemId);
        if (item is null)
        {
            return false;
        }

        _items.Remove(item);
        UpdatedAtUtc = nowUtc;

        return true;
    }

    public void Clear(DateTime nowUtc)
    {
        _items.Clear();
        UpdatedAtUtc = nowUtc;
    }
}

namespace DropUz.Modules.Cart.Application.Carts;

public sealed record CartResponse(
    Guid? CartId,
    Guid? SellerId,
    IReadOnlyCollection<CartItemResponse> Items,
    decimal ProductTotal,
    string CargoStatus);

public sealed record CartItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string? ProductImageUrl,
    int Quantity,
    decimal UnitPrice,
    decimal SellerProfit,
    decimal LineTotal);

namespace DropUz.Modules.CoreMvp;

public sealed record CreateProductRequest(
    string Name,
    string Description,
    string SourceUrl,
    decimal SourcePrice,
    decimal DropUzMarkupPercent);

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    string SourceUrl,
    decimal SourcePrice,
    decimal DropUzMarkupPercent,
    decimal Price,
    bool IsApproved);

public sealed record CreateSellerShopRequest(
    string Name,
    string Slug,
    decimal GlobalMarkupPercent);

public sealed record SellerShopResponse(
    Guid Id,
    string Name,
    string Slug,
    decimal GlobalMarkupPercent);

public sealed record AddSellerProductRequest(
    Guid ProductId,
    decimal MarkupPercent);

public sealed record SellerProductResponse(
    Guid Id,
    Guid SellerShopId,
    Guid ProductId,
    decimal MarkupPercent,
    decimal SellerPrice);

public sealed record AddCartItemRequest(
    Guid ProductId,
    Guid? SellerShopId,
    int Quantity);

public sealed record UpdateCartItemRequest(int Quantity);

public sealed record CartResponse(
    IReadOnlyCollection<CartItemResponse> Items,
    decimal ProductTotal);

public sealed record CartItemResponse(
    Guid Id,
    Guid ProductId,
    Guid? SellerShopId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

public sealed record OrderResponse(
    Guid Id,
    string Status,
    decimal ProductTotal,
    decimal? CargoPrice,
    DateTimeOffset? CargoPaymentDeadlineUtc,
    IReadOnlyCollection<OrderItemResponse> Items);

public sealed record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    Guid? SellerShopId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

public sealed record PayOrderRequest(Guid OrderId);

public sealed record PaymentResponse(
    Guid Id,
    Guid OrderId,
    string Kind,
    decimal Amount,
    string Status,
    string OrderStatus);

public sealed record SetCargoPriceRequest(decimal Amount);

public sealed record CargoResponse(
    Guid OrderId,
    string OrderStatus,
    decimal CargoPrice,
    DateTimeOffset CargoPaymentDeadlineUtc);

public sealed record UpdateOrderStatusRequest(string Status);

public sealed record NotificationResponse(
    Guid Id,
    Guid? OrderId,
    string Type,
    string Message,
    DateTimeOffset CreatedAtUtc);

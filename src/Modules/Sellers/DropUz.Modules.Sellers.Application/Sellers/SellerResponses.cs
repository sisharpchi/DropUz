using DropUz.Modules.Catalog.Domain.Pricing;

namespace DropUz.Modules.Sellers.Application.Sellers;

public sealed record SellerProfileResponse(
    Guid Id,
    Guid UserId,
    string ShopName,
    string Slug,
    Markup GlobalMarkup);

public sealed record SellerProductResponse(
    Guid Id,
    Guid SellerId,
    Guid ProductId,
    string ProductName,
    string? ProductImageUrl,
    Markup? ProductMarkup,
    decimal DropUzFinalPrice,
    Markup AppliedSellerMarkup,
    decimal SellerProfit,
    decimal FinalPrice,
    bool IsActive);

public sealed record SellerBalanceResponse(
    Guid SellerId,
    string ShopName,
    decimal PendingBalance,
    decimal AvailableBalance,
    decimal WithdrawnBalance,
    decimal TotalEarned);

public sealed record SellerPriceQuote(
    Guid SellerId,
    Guid ProductId,
    Guid SellerProductId,
    decimal DropUzFinalPrice,
    Markup AppliedSellerMarkup,
    decimal SellerProfit,
    decimal FinalPrice);

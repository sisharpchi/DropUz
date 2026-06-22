using DropUz.Modules.Catalog.Domain.Pricing;

namespace DropUz.Modules.Orders.Domain.Orders;

public sealed record OrderItemSnapshot(
    Guid ProductId,
    string ProductName,
    string? ProductImageUrl,
    string? VariantName,
    string SourcePlatform,
    string SourceProductId,
    string? SourceUrl,
    decimal ApiPrice,
    decimal CurrencyRate,
    Markup DropUzMarkup,
    decimal DropUzMarkupAmount,
    decimal DropUzFinalPrice,
    Guid? SellerId,
    Markup? SellerMarkup,
    decimal SellerProfit,
    decimal FinalProductPrice,
    decimal CargoPrice,
    int Quantity)
{
    public decimal ProductLineTotal => decimal.Round(FinalProductPrice * Quantity, 2, MidpointRounding.AwayFromZero);

    public decimal SellerProfitTotal => decimal.Round(SellerProfit * Quantity, 2, MidpointRounding.AwayFromZero);
}

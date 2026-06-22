using DropUz.Common.Domain;
using DropUz.Modules.Catalog.Domain.Pricing;

namespace DropUz.Modules.Orders.Domain.Orders;

public sealed class OrderItem : Entity
{
    private OrderItem()
    {
    }

    internal OrderItem(Guid id, Guid orderId, OrderItemSnapshot snapshot)
        : base(id)
    {
        OrderId = orderId;
        ProductId = snapshot.ProductId;
        ProductName = snapshot.ProductName;
        ProductImageUrl = snapshot.ProductImageUrl;
        VariantName = snapshot.VariantName;
        SourcePlatform = snapshot.SourcePlatform;
        SourceProductId = snapshot.SourceProductId;
        SourceUrl = snapshot.SourceUrl;
        ApiPrice = snapshot.ApiPrice;
        CurrencyRate = snapshot.CurrencyRate;
        DropUzMarkupType = snapshot.DropUzMarkup.Type;
        DropUzMarkupValue = snapshot.DropUzMarkup.Value;
        DropUzMarkupAmount = snapshot.DropUzMarkupAmount;
        DropUzFinalPrice = snapshot.DropUzFinalPrice;
        SellerId = snapshot.SellerId;
        SellerMarkupType = snapshot.SellerMarkup?.Type;
        SellerMarkupValue = snapshot.SellerMarkup?.Value;
        SellerProfit = snapshot.SellerProfit;
        FinalProductPrice = snapshot.FinalProductPrice;
        CargoPrice = snapshot.CargoPrice;
        Quantity = snapshot.Quantity;
        ProductLineTotal = snapshot.ProductLineTotal;
        SellerProfitTotal = snapshot.SellerProfitTotal;
        Total = ProductLineTotal + CargoPrice;
    }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public string? ProductImageUrl { get; private set; }

    public string? VariantName { get; private set; }

    public string SourcePlatform { get; private set; } = string.Empty;

    public string SourceProductId { get; private set; } = string.Empty;

    public string? SourceUrl { get; private set; }

    public decimal ApiPrice { get; private set; }

    public decimal CurrencyRate { get; private set; }

    public MarkupType DropUzMarkupType { get; private set; }

    public decimal DropUzMarkupValue { get; private set; }

    public decimal DropUzMarkupAmount { get; private set; }

    public decimal DropUzFinalPrice { get; private set; }

    public Guid? SellerId { get; private set; }

    public MarkupType? SellerMarkupType { get; private set; }

    public decimal? SellerMarkupValue { get; private set; }

    public decimal SellerProfit { get; private set; }

    public decimal FinalProductPrice { get; private set; }

    public decimal CargoPrice { get; private set; }

    public int Quantity { get; private set; }

    public decimal ProductLineTotal { get; private set; }

    public decimal SellerProfitTotal { get; private set; }

    public decimal Total { get; private set; }

    public void SetCargoPrice(decimal cargoPrice)
    {
        CargoPrice = cargoPrice;
        Total = ProductLineTotal + CargoPrice;
    }
}

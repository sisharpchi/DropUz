using DropUz.Modules.Catalog.Domain.Pricing;

namespace DropUz.Modules.Sellers.Domain.Pricing;

public sealed record SellerPriceBreakdown(
    Markup AppliedMarkup,
    decimal SellerProfit,
    decimal FinalPrice);

public static class SellerPriceCalculator
{
    public static SellerPriceBreakdown Calculate(
        decimal dropUzFinalPrice,
        Markup sellerGlobalMarkup,
        Markup? sellerProductMarkup)
    {
        Markup appliedMarkup = sellerProductMarkup ?? sellerGlobalMarkup;
        decimal sellerProfit = DropUzPriceCalculator.CalculateMarkupAmount(dropUzFinalPrice, appliedMarkup);

        return new SellerPriceBreakdown(
            appliedMarkup,
            sellerProfit,
            decimal.Round(dropUzFinalPrice + sellerProfit, 2, MidpointRounding.AwayFromZero));
    }
}

using DropUz.Modules.Catalog.Domain.Pricing;
using DropUz.Modules.Sellers.Domain.Pricing;
using Xunit;

namespace DropUz.Mvp.Tests.Pricing;

public sealed class MarkupCalculatorTests
{
    [Fact]
    public void ProductSpecificDropUzMarkupOverridesGlobalMarkup()
    {
        var result = DropUzPriceCalculator.Calculate(
            apiPrice: 100m,
            globalMarkup: new Markup(MarkupType.Percent, 10m),
            productMarkup: new Markup(MarkupType.Fixed, 25m));

        Assert.Equal(25m, result.MarkupAmount);
        Assert.Equal(125m, result.FinalPrice);
        Assert.Equal(MarkupType.Fixed, result.AppliedMarkup.Type);
    }

    [Fact]
    public void SellerProductMarkupOverridesSellerGlobalMarkup()
    {
        var result = SellerPriceCalculator.Calculate(
            dropUzFinalPrice: 125m,
            sellerGlobalMarkup: new Markup(MarkupType.Percent, 10m),
            sellerProductMarkup: new Markup(MarkupType.Fixed, 20m));

        Assert.Equal(20m, result.SellerProfit);
        Assert.Equal(145m, result.FinalPrice);
        Assert.Equal(MarkupType.Fixed, result.AppliedMarkup.Type);
    }
}

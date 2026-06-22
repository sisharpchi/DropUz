namespace DropUz.Modules.Catalog.Domain.Pricing;

public static class DropUzPriceCalculator
{
    public static PriceBreakdown Calculate(
        decimal apiPrice,
        Markup globalMarkup,
        Markup? productMarkup)
    {
        Markup appliedMarkup = productMarkup ?? globalMarkup;
        decimal markupAmount = CalculateMarkupAmount(apiPrice, appliedMarkup);

        return new PriceBreakdown(
            appliedMarkup,
            markupAmount,
            decimal.Round(apiPrice + markupAmount, 2, MidpointRounding.AwayFromZero));
    }

    public static decimal CalculateMarkupAmount(decimal basePrice, Markup markup)
    {
        decimal amount = markup.Type switch
        {
            MarkupType.Percent => basePrice * markup.Value / 100m,
            MarkupType.Fixed => markup.Value,
            _ => 0m
        };

        return decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }
}

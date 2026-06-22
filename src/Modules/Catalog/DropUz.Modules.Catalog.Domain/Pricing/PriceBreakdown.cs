namespace DropUz.Modules.Catalog.Domain.Pricing;

public sealed record PriceBreakdown(
    Markup AppliedMarkup,
    decimal MarkupAmount,
    decimal FinalPrice);

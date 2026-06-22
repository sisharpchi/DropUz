using DropUz.Modules.Catalog.Domain.Pricing;

namespace DropUz.Modules.Catalog.Application.Products;

public sealed record CatalogPriceQuote(
    Guid ProductId,
    decimal ApiPrice,
    decimal CurrencyRate,
    Markup AppliedMarkup,
    decimal MarkupAmount,
    decimal DropUzFinalPrice);

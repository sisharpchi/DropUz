using DropUz.Modules.Catalog.Domain.Pricing;
using DropUz.Modules.Catalog.Domain.Products;

namespace DropUz.Modules.Catalog.Application.Products;

public sealed record CatalogProductResponse(
    Guid Id,
    Guid? CategoryId,
    string Name,
    string? Description,
    string? ImageUrl,
    string SourcePlatform,
    string SourceProductId,
    string? SourceUrl,
    decimal ApiPrice,
    string CurrencyCode,
    decimal CurrencyRate,
    ProductStatus Status,
    Markup? ProductMarkup,
    Markup AppliedDropUzMarkup,
    decimal DropUzMarkupAmount,
    decimal DropUzFinalPrice);

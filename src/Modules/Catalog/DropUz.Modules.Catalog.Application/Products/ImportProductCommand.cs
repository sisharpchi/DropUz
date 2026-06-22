using DropUz.Common.Application.Messaging;

namespace DropUz.Modules.Catalog.Application.Products;

public sealed record ImportProductCommand(
    Guid? CategoryId,
    string Name,
    string? Description,
    string? ImageUrl,
    string SourcePlatform,
    string SourceProductId,
    string? SourceUrl,
    decimal ApiPrice,
    string CurrencyCode,
    decimal CurrencyRate) : ICommand<CatalogProductResponse>;

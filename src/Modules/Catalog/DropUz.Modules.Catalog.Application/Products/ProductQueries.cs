using DropUz.Common.Application.Messaging;

namespace DropUz.Modules.Catalog.Application.Products;

public sealed record GetCatalogProductsQuery(
    string? Search,
    Guid? CategoryId,
    bool ApprovedOnly) : IQuery<IReadOnlyCollection<CatalogProductResponse>>;

public sealed record GetCatalogProductQuery(Guid ProductId) : IQuery<CatalogProductResponse>;

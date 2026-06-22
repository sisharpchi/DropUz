using DropUz.Common.Domain;

namespace DropUz.Modules.Catalog.Application.Products;

public interface ICatalogPricingService
{
    Task<Result<CatalogPriceQuote>> CalculateDropUzPriceAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}

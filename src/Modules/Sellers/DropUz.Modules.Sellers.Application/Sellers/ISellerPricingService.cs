using DropUz.Common.Domain;

namespace DropUz.Modules.Sellers.Application.Sellers;

public interface ISellerPricingService
{
    Task<Result<SellerPriceQuote>> CalculateSellerPriceAsync(
        Guid sellerId,
        Guid productId,
        CancellationToken cancellationToken = default);
}

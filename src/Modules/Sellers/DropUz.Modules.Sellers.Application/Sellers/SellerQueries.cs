using DropUz.Common.Application.Messaging;

namespace DropUz.Modules.Sellers.Application.Sellers;

public sealed record GetMySellerProfileQuery : IQuery<SellerProfileResponse>;

public sealed record GetSellerBalanceQuery(Guid? SellerId) : IQuery<SellerBalanceResponse>;

public sealed record GetSellerBalancesQuery : IQuery<IReadOnlyCollection<SellerBalanceResponse>>;

public sealed record GetShopProductsQuery(string Slug) : IQuery<IReadOnlyCollection<SellerProductResponse>>;

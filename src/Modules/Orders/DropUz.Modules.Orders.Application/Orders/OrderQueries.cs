using DropUz.Common.Application.Messaging;
using DropUz.Modules.Orders.Domain.Orders;

namespace DropUz.Modules.Orders.Application.Orders;

public sealed record GetMyOrdersQuery : IQuery<IReadOnlyCollection<OrderResponse>>;

public sealed record GetSellerOrdersQuery : IQuery<IReadOnlyCollection<OrderResponse>>;

public sealed record GetOrderQuery(Guid OrderId) : IQuery<OrderResponse>;

public sealed record GetAdminOrdersQuery(OrderStatus? Status) : IQuery<IReadOnlyCollection<OrderResponse>>;

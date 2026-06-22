using DropUz.Common.Application.Messaging;
using DropUz.Modules.Orders.Domain.Orders;

namespace DropUz.Modules.Orders.Application.Orders;

public sealed record CreateOrderFromCartCommand(Guid? SellerId) : ICommand<OrderResponse>;

public sealed record AdminSetCargoPriceCommand(Guid OrderId, decimal CargoPrice, int? DeadlineDays) : ICommand<OrderResponse>;

public sealed record AdminUpdateOrderStatusCommand(Guid OrderId, OrderStatus Status, string? Note) : ICommand<OrderResponse>;

public sealed record ExpireCargoPaymentsCommand : ICommand<int>;

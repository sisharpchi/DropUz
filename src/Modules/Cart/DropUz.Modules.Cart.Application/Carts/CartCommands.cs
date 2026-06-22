using DropUz.Common.Application.Messaging;

namespace DropUz.Modules.Cart.Application.Carts;

public sealed record AddCartItemCommand(Guid ProductId, Guid? SellerId, int Quantity) : ICommand<CartResponse>;

public sealed record UpdateCartItemCommand(Guid CartItemId, Guid? SellerId, int Quantity) : ICommand<CartResponse>;

public sealed record RemoveCartItemCommand(Guid CartItemId, Guid? SellerId) : ICommand<CartResponse>;

public sealed record ClearCartCommand(Guid? SellerId) : ICommand<CartResponse>;

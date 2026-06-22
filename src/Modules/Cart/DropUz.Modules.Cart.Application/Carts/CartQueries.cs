using DropUz.Common.Application.Messaging;

namespace DropUz.Modules.Cart.Application.Carts;

public sealed record GetMyCartQuery(Guid? SellerId) : IQuery<CartResponse>;

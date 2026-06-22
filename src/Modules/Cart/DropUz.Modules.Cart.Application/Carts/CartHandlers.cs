using DropUz.Common.Application.Abstractions;
using DropUz.Common.Application.Clock;
using DropUz.Common.Application.Data;
using DropUz.Common.Application.Messaging;
using DropUz.Common.Domain;
using DropUz.Modules.Cart.Domain.Carts;
using DropUz.Modules.Catalog.Application.Products;
using DropUz.Modules.Catalog.Domain.Products;
using DropUz.Modules.Sellers.Application.Sellers;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Modules.Cart.Application.Carts;

public sealed class AddCartItemCommandHandler(
    IMainRepository repository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    ICatalogPricingService catalogPricingService,
    ISellerPricingService sellerPricingService)
    : ICommandHandler<AddCartItemCommand, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(
        AddCartItemCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Quantity <= 0)
        {
            return Result.Failure<CartResponse>(CartErrors.QuantityInvalid);
        }

        Result<ShoppingCart> cartResult = await CartMapper.GetOrCreateCartAsync(
            repository,
            currentUser,
            command.SellerId,
            dateTimeProvider.UtcNow,
            cancellationToken);

        if (cartResult.IsFailure)
        {
            return Result.Failure<CartResponse>(cartResult.Error);
        }

        CatalogProduct? product = await repository.GetAsync<CatalogProduct>(command.ProductId);
        if (product is null)
        {
            return Result.Failure<CartResponse>(DropUz.Modules.Catalog.Application.CatalogErrors.ProductNotFound);
        }

        cartResult.Value.AddOrUpdateItem(command.ProductId, command.Quantity, dateTimeProvider.UtcNow);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return await CartMapper.MapAsync(
            repository,
            catalogPricingService,
            sellerPricingService,
            cartResult.Value,
            cancellationToken);
    }
}

public sealed class UpdateCartItemCommandHandler(
    IMainRepository repository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    ICatalogPricingService catalogPricingService,
    ISellerPricingService sellerPricingService)
    : ICommandHandler<UpdateCartItemCommand, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(
        UpdateCartItemCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Quantity <= 0)
        {
            return Result.Failure<CartResponse>(CartErrors.QuantityInvalid);
        }

        Result<ShoppingCart> cartResult = await CartMapper.GetCartAsync(
            repository,
            currentUser,
            command.SellerId,
            cancellationToken);

        if (cartResult.IsFailure)
        {
            return Result.Failure<CartResponse>(cartResult.Error);
        }

        if (!cartResult.Value.UpdateItem(command.CartItemId, command.Quantity, dateTimeProvider.UtcNow))
        {
            return Result.Failure<CartResponse>(CartErrors.CartItemNotFound);
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return await CartMapper.MapAsync(repository, catalogPricingService, sellerPricingService, cartResult.Value, cancellationToken);
    }
}

public sealed class RemoveCartItemCommandHandler(
    IMainRepository repository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    ICatalogPricingService catalogPricingService,
    ISellerPricingService sellerPricingService)
    : ICommandHandler<RemoveCartItemCommand, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(
        RemoveCartItemCommand command,
        CancellationToken cancellationToken)
    {
        Result<ShoppingCart> cartResult = await CartMapper.GetCartAsync(repository, currentUser, command.SellerId, cancellationToken);
        if (cartResult.IsFailure)
        {
            return Result.Failure<CartResponse>(cartResult.Error);
        }

        if (!cartResult.Value.RemoveItem(command.CartItemId, dateTimeProvider.UtcNow))
        {
            return Result.Failure<CartResponse>(CartErrors.CartItemNotFound);
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return await CartMapper.MapAsync(repository, catalogPricingService, sellerPricingService, cartResult.Value, cancellationToken);
    }
}

public sealed class ClearCartCommandHandler(
    IMainRepository repository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider,
    ICatalogPricingService catalogPricingService,
    ISellerPricingService sellerPricingService)
    : ICommandHandler<ClearCartCommand, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(
        ClearCartCommand command,
        CancellationToken cancellationToken)
    {
        Result<ShoppingCart> cartResult = await CartMapper.GetCartAsync(repository, currentUser, command.SellerId, cancellationToken);
        if (cartResult.IsFailure)
        {
            return Result.Failure<CartResponse>(cartResult.Error);
        }

        cartResult.Value.Clear(dateTimeProvider.UtcNow);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return await CartMapper.MapAsync(repository, catalogPricingService, sellerPricingService, cartResult.Value, cancellationToken);
    }
}

public sealed class GetMyCartQueryHandler(
    IMainRepository repository,
    ICurrentUser currentUser,
    ICatalogPricingService catalogPricingService,
    ISellerPricingService sellerPricingService)
    : IQueryHandler<GetMyCartQuery, CartResponse>
{
    public async Task<Result<CartResponse>> Handle(
        GetMyCartQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result.Failure<CartResponse>(CartErrors.UserNotAuthenticated);
        }

        ShoppingCart? cart = await CartMapper.FindCartAsync(
            repository,
            currentUser.UserId.Value,
            request.SellerId,
            cancellationToken);

        if (cart is null)
        {
            return Result.Success(new CartResponse(null, request.SellerId, [], 0m, "Calculating"));
        }

        return await CartMapper.MapAsync(repository, catalogPricingService, sellerPricingService, cart, cancellationToken);
    }
}

public static class CartMapper
{
    public static async Task<ShoppingCart?> FindCartAsync(
        IMainRepository repository,
        Guid userId,
        Guid? sellerId,
        CancellationToken cancellationToken)
    {
        return await repository
            .Query<ShoppingCart>(cart => cart.UserId == userId && cart.SellerId == sellerId)
            .Include(cart => cart.Items)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public static async Task<Result<ShoppingCart>> GetOrCreateCartAsync(
        IMainRepository repository,
        ICurrentUser currentUser,
        Guid? sellerId,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result.Failure<ShoppingCart>(CartErrors.UserNotAuthenticated);
        }

        ShoppingCart? cart = await FindCartAsync(repository, currentUser.UserId.Value, sellerId, cancellationToken);
        if (cart is not null)
        {
            return Result.Success(cart);
        }

        cart = ShoppingCart.Create(currentUser.UserId.Value, sellerId, nowUtc);
        await repository.AddAsync(cart);

        return Result.Success(cart);
    }

    public static async Task<Result<ShoppingCart>> GetCartAsync(
        IMainRepository repository,
        ICurrentUser currentUser,
        Guid? sellerId,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result.Failure<ShoppingCart>(CartErrors.UserNotAuthenticated);
        }

        ShoppingCart? cart = await FindCartAsync(repository, currentUser.UserId.Value, sellerId, cancellationToken);

        return cart is null
            ? Result.Failure<ShoppingCart>(CartErrors.CartNotFound)
            : Result.Success(cart);
    }

    public static async Task<Result<CartResponse>> MapAsync(
        IMainRepository repository,
        ICatalogPricingService catalogPricingService,
        ISellerPricingService sellerPricingService,
        ShoppingCart cart,
        CancellationToken cancellationToken)
    {
        var items = new List<CartItemResponse>();

        foreach (CartItem cartItem in cart.Items.OrderBy(item => item.CreatedAtUtc))
        {
            CatalogProduct? product = await repository.GetAsync<CatalogProduct>(cartItem.ProductId);
            if (product is null)
            {
                continue;
            }

            decimal unitPrice;
            decimal sellerProfit;

            if (cart.SellerId.HasValue)
            {
                Result<SellerPriceQuote> sellerPrice = await sellerPricingService.CalculateSellerPriceAsync(
                    cart.SellerId.Value,
                    cartItem.ProductId,
                    cancellationToken);

                if (sellerPrice.IsFailure)
                {
                    continue;
                }

                unitPrice = sellerPrice.Value.FinalPrice;
                sellerProfit = sellerPrice.Value.SellerProfit;
            }
            else
            {
                Result<CatalogPriceQuote> catalogPrice = await catalogPricingService.CalculateDropUzPriceAsync(
                    cartItem.ProductId,
                    cancellationToken);

                if (catalogPrice.IsFailure)
                {
                    continue;
                }

                unitPrice = catalogPrice.Value.DropUzFinalPrice;
                sellerProfit = 0m;
            }

            items.Add(new CartItemResponse(
                cartItem.Id,
                cartItem.ProductId,
                product.Name,
                product.ImageUrl,
                cartItem.Quantity,
                unitPrice,
                sellerProfit,
                decimal.Round(unitPrice * cartItem.Quantity, 2, MidpointRounding.AwayFromZero)));
        }

        return Result.Success(new CartResponse(
            cart.Id,
            cart.SellerId,
            items,
            items.Sum(item => item.LineTotal),
            "Calculating"));
    }
}

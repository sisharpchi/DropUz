using DropUz.Common.Application.Abstractions;
using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Modules.CoreMvp;

public sealed class CartCoreMvpEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/cart")
            .RequireAuthorization()
            .WithTags("Cart");

        group.MapGet("/", GetCartAsync);
        group.MapPost("/items", AddItemAsync);
        group.MapPut("/items/{itemId:guid}", UpdateItemAsync);
        group.MapDelete("/items/{itemId:guid}", RemoveItemAsync);
    }

    private static async Task<IResult> GetCartAsync(
        ICurrentUser currentUser,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (!CoreMvpEndpointHelpers.TryGetCurrentUserId(currentUser, out Guid userId))
        {
            return CoreMvpEndpointHelpers.UnauthorizedCurrentUser();
        }

        IReadOnlyCollection<MvpCartItem> items = await GetItemsAsync(context, userId, cancellationToken);

        return TypedResults.Ok(CoreMvpEndpointHelpers.ToCartResponse(items));
    }

    private static async Task<IResult> AddItemAsync(
        AddCartItemRequest request,
        ICurrentUser currentUser,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (!CoreMvpEndpointHelpers.TryGetCurrentUserId(currentUser, out Guid userId))
        {
            return CoreMvpEndpointHelpers.UnauthorizedCurrentUser();
        }

        if (request.Quantity <= 0)
        {
            return Results.Problem("Quantity must be positive.", statusCode: StatusCodes.Status400BadRequest);
        }

        MvpProduct? product = await context
            .Set<MvpProduct>()
            .FirstOrDefaultAsync(item => item.Id == request.ProductId && item.IsApproved, cancellationToken);

        if (product is null)
        {
            return Results.Problem("Approved product was not found.", statusCode: StatusCodes.Status400BadRequest);
        }

        decimal unitPrice = product.Price;

        if (request.SellerShopId is Guid sellerShopId)
        {
            MvpSellerProduct? sellerProduct = await context
                .Set<MvpSellerProduct>()
                .FirstOrDefaultAsync(item => item.SellerShopId == sellerShopId && item.ProductId == product.Id, cancellationToken);

            if (sellerProduct is null)
            {
                return Results.Problem("Seller product was not found.", statusCode: StatusCodes.Status400BadRequest);
            }

            unitPrice = sellerProduct.SellerPrice;
        }

        var cartItem = new MvpCartItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = product.Id,
            SellerShopId = request.SellerShopId,
            ProductName = product.Name,
            UnitPrice = unitPrice,
            Quantity = request.Quantity,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        context.Set<MvpCartItem>().Add(cartItem);
        await context.SaveChangesAsync(cancellationToken);

        IReadOnlyCollection<MvpCartItem> items = await GetItemsAsync(context, userId, cancellationToken);

        return TypedResults.Created($"/api/cart/items/{cartItem.Id}", CoreMvpEndpointHelpers.ToCartResponse(items));
    }

    private static async Task<IResult> UpdateItemAsync(
        Guid itemId,
        UpdateCartItemRequest request,
        ICurrentUser currentUser,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (!CoreMvpEndpointHelpers.TryGetCurrentUserId(currentUser, out Guid userId))
        {
            return CoreMvpEndpointHelpers.UnauthorizedCurrentUser();
        }

        if (request.Quantity <= 0)
        {
            return Results.Problem("Quantity must be positive.", statusCode: StatusCodes.Status400BadRequest);
        }

        MvpCartItem? cartItem = await context
            .Set<MvpCartItem>()
            .FirstOrDefaultAsync(item => item.Id == itemId && item.UserId == userId, cancellationToken);

        if (cartItem is null)
        {
            return Results.NotFound();
        }

        cartItem.Quantity = request.Quantity;
        await context.SaveChangesAsync(cancellationToken);

        IReadOnlyCollection<MvpCartItem> items = await GetItemsAsync(context, userId, cancellationToken);

        return TypedResults.Ok(CoreMvpEndpointHelpers.ToCartResponse(items));
    }

    private static async Task<IResult> RemoveItemAsync(
        Guid itemId,
        ICurrentUser currentUser,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (!CoreMvpEndpointHelpers.TryGetCurrentUserId(currentUser, out Guid userId))
        {
            return CoreMvpEndpointHelpers.UnauthorizedCurrentUser();
        }

        MvpCartItem? cartItem = await context
            .Set<MvpCartItem>()
            .FirstOrDefaultAsync(item => item.Id == itemId && item.UserId == userId, cancellationToken);

        if (cartItem is null)
        {
            return Results.NotFound();
        }

        context.Set<MvpCartItem>().Remove(cartItem);
        await context.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }

    private static async Task<IReadOnlyCollection<MvpCartItem>> GetItemsAsync(
        MainDbContext context,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return await context
            .Set<MvpCartItem>()
            .Where(item => item.UserId == userId)
            .OrderBy(item => item.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);
    }
}

using DropUz.Common.Application.Abstractions;
using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Modules.CoreMvp;

public sealed class OrdersCoreMvpEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/orders")
            .RequireAuthorization()
            .WithTags("Orders");

        group.MapPost("/from-cart", CreateFromCartAsync);
        group.MapGet("/", ListAsync);
        group.MapGet("/{orderId:guid}", GetAsync);
    }

    private static async Task<IResult> CreateFromCartAsync(
        ICurrentUser currentUser,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (!CoreMvpEndpointHelpers.TryGetCurrentUserId(currentUser, out Guid userId))
        {
            return CoreMvpEndpointHelpers.UnauthorizedCurrentUser();
        }

        MvpCartItem[] cartItems = await context
            .Set<MvpCartItem>()
            .Where(item => item.UserId == userId)
            .OrderBy(item => item.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        if (cartItems.Length == 0)
        {
            return Results.Problem("Cart is empty.", statusCode: StatusCodes.Status400BadRequest);
        }

        var order = new MvpOrder
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = OrderStatuses.PendingProductPayment,
            ProductTotal = CoreMvpEndpointHelpers.Money(cartItems.Sum(item => item.UnitPrice * item.Quantity)),
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        MvpOrderItem[] orderItems = cartItems
            .Select(item => new MvpOrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = item.ProductId,
                SellerShopId = item.SellerShopId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                LineTotal = CoreMvpEndpointHelpers.Money(item.UnitPrice * item.Quantity)
            })
            .ToArray();

        context.Set<MvpOrder>().Add(order);
        context.Set<MvpOrderItem>().AddRange(orderItems);
        context.Set<MvpCartItem>().RemoveRange(cartItems);
        CoreMvpEndpointHelpers.AddNotification(
            context,
            userId,
            order.Id,
            "OrderCreated",
            "Order was created and is waiting for product payment.");

        await context.SaveChangesAsync(cancellationToken);

        OrderResponse response = await order.ToOrderResponseAsync(context, cancellationToken);

        return TypedResults.Created($"/api/orders/{order.Id}", response);
    }

    private static async Task<IResult> ListAsync(
        ICurrentUser currentUser,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (!CoreMvpEndpointHelpers.TryGetCurrentUserId(currentUser, out Guid userId))
        {
            return CoreMvpEndpointHelpers.UnauthorizedCurrentUser();
        }

        MvpOrder[] orders = await context
            .Set<MvpOrder>()
            .Where(order => order.UserId == userId)
            .OrderByDescending(order => order.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        var responses = new List<OrderResponse>(orders.Length);

        foreach (MvpOrder order in orders)
        {
            responses.Add(await order.ToOrderResponseAsync(context, cancellationToken));
        }

        return TypedResults.Ok(responses);
    }

    private static async Task<IResult> GetAsync(
        Guid orderId,
        ICurrentUser currentUser,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (!CoreMvpEndpointHelpers.TryGetCurrentUserId(currentUser, out Guid userId))
        {
            return CoreMvpEndpointHelpers.UnauthorizedCurrentUser();
        }

        MvpOrder? order = await context
            .Set<MvpOrder>()
            .FirstOrDefaultAsync(item => item.Id == orderId && item.UserId == userId, cancellationToken);

        return order is null
            ? Results.NotFound()
            : TypedResults.Ok(await order.ToOrderResponseAsync(context, cancellationToken));
    }
}

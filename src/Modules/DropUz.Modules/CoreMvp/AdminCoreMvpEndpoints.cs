using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Modules.CoreMvp;

public sealed class AdminCoreMvpEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/admin")
            .RequireAuthorization()
            .WithTags("Admin");

        group.MapGet("/orders", ListOrdersAsync);
        group.MapPost("/orders/{orderId:guid}/status", UpdateStatusAsync);
    }

    private static async Task<IResult> ListOrdersAsync(
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        MvpOrder[] orders = await context
            .Set<MvpOrder>()
            .OrderByDescending(order => order.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        var responses = new List<OrderResponse>(orders.Length);

        foreach (MvpOrder order in orders)
        {
            responses.Add(await order.ToOrderResponseAsync(context, cancellationToken));
        }

        return TypedResults.Ok(responses);
    }

    private static async Task<IResult> UpdateStatusAsync(
        Guid orderId,
        UpdateOrderStatusRequest request,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Status))
        {
            return Results.Problem("Status is required.", statusCode: StatusCodes.Status400BadRequest);
        }

        MvpOrder? order = await context
            .Set<MvpOrder>()
            .FirstOrDefaultAsync(item => item.Id == orderId, cancellationToken);

        if (order is null)
        {
            return Results.NotFound();
        }

        order.Status = request.Status.Trim();

        if (string.Equals(order.Status, OrderStatuses.Delivered, StringComparison.OrdinalIgnoreCase))
        {
            order.Status = OrderStatuses.Delivered;
            order.DeliveredAtUtc = DateTimeOffset.UtcNow;
        }

        CoreMvpEndpointHelpers.AddNotification(
            context,
            order.UserId,
            order.Id,
            "OrderStatusChanged",
            $"Order status changed to {order.Status}.");

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(await order.ToOrderResponseAsync(context, cancellationToken));
    }
}

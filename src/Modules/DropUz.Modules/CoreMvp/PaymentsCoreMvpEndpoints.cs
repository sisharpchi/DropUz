using DropUz.Common.Application.Abstractions;
using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Modules.CoreMvp;

public sealed class PaymentsCoreMvpEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/payments")
            .RequireAuthorization()
            .WithTags("Payments");

        group.MapPost("/product", PayProductAsync);
        group.MapPost("/cargo", PayCargoAsync);
    }

    private static async Task<IResult> PayProductAsync(
        PayOrderRequest request,
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
            .FirstOrDefaultAsync(item => item.Id == request.OrderId && item.UserId == userId, cancellationToken);

        if (order is null)
        {
            return Results.NotFound();
        }

        if (order.Status != OrderStatuses.PendingProductPayment)
        {
            return Results.Problem("Order is not waiting for product payment.", statusCode: StatusCodes.Status400BadRequest);
        }

        var payment = CreatePayment(order.Id, PaymentKinds.Product, order.ProductTotal);
        order.Status = OrderStatuses.ProductPaid;
        order.ProductPaidAtUtc = payment.PaidAtUtc;

        context.Set<MvpPayment>().Add(payment);
        CoreMvpEndpointHelpers.AddNotification(
            context,
            userId,
            order.Id,
            "ProductPaid",
            "Product payment was received.");

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ToResponse(payment, order.Status));
    }

    private static async Task<IResult> PayCargoAsync(
        PayOrderRequest request,
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
            .FirstOrDefaultAsync(item => item.Id == request.OrderId && item.UserId == userId, cancellationToken);

        if (order is null)
        {
            return Results.NotFound();
        }

        if (order.Status != OrderStatuses.PendingCargoPayment || order.CargoPrice is null)
        {
            return Results.Problem("Order is not waiting for cargo payment.", statusCode: StatusCodes.Status400BadRequest);
        }

        var payment = CreatePayment(order.Id, PaymentKinds.Cargo, order.CargoPrice.Value);
        order.Status = OrderStatuses.CargoPaid;
        order.CargoPaidAtUtc = payment.PaidAtUtc;

        context.Set<MvpPayment>().Add(payment);
        CoreMvpEndpointHelpers.AddNotification(
            context,
            userId,
            order.Id,
            "CargoPaid",
            "Cargo payment was received.");

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(ToResponse(payment, order.Status));
    }

    private static MvpPayment CreatePayment(Guid orderId, string kind, decimal amount)
    {
        return new MvpPayment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Kind = kind,
            Amount = CoreMvpEndpointHelpers.Money(amount),
            Status = PaymentStatuses.Paid,
            PaidAtUtc = DateTimeOffset.UtcNow
        };
    }

    private static PaymentResponse ToResponse(MvpPayment payment, string orderStatus)
    {
        return new PaymentResponse(
            payment.Id,
            payment.OrderId,
            payment.Kind,
            payment.Amount,
            payment.Status,
            orderStatus);
    }
}

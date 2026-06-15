using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Modules.CoreMvp;

public sealed class CargoCoreMvpEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/cargo")
            .RequireAuthorization()
            .WithTags("Cargo");

        group.MapPost("/orders/{orderId:guid}/price", SetPriceAsync);
    }

    private static async Task<IResult> SetPriceAsync(
        Guid orderId,
        SetCargoPriceRequest request,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
        {
            return Results.Problem("Cargo amount must be positive.", statusCode: StatusCodes.Status400BadRequest);
        }

        MvpOrder? order = await context
            .Set<MvpOrder>()
            .FirstOrDefaultAsync(item => item.Id == orderId, cancellationToken);

        if (order is null)
        {
            return Results.NotFound();
        }

        if (order.Status != OrderStatuses.ProductPaid)
        {
            return Results.Problem("Cargo price can be entered after product payment.", statusCode: StatusCodes.Status400BadRequest);
        }

        DateTimeOffset deadline = DateTimeOffset.UtcNow.AddDays(7);

        order.CargoPrice = CoreMvpEndpointHelpers.Money(request.Amount);
        order.CargoPaymentDeadlineUtc = deadline;
        order.Status = OrderStatuses.PendingCargoPayment;

        CoreMvpEndpointHelpers.AddNotification(
            context,
            order.UserId,
            order.Id,
            "CargoPriceEntered",
            "Cargo price was entered and is waiting for payment.");

        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(new CargoResponse(
            order.Id,
            order.Status,
            order.CargoPrice.Value,
            deadline));
    }
}

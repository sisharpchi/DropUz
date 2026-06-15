using DropUz.Common.Application.Abstractions;
using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Modules.CoreMvp;

public sealed class NotificationsCoreMvpEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/notifications")
            .RequireAuthorization()
            .WithTags("Notifications");

        group.MapGet("/", ListAsync);
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

        NotificationResponse[] notifications = await context
            .Set<MvpNotification>()
            .Where(notification => notification.UserId == userId)
            .OrderByDescending(notification => notification.CreatedAtUtc)
            .Select(notification => notification.ToResponse())
            .ToArrayAsync(cancellationToken);

        return TypedResults.Ok(notifications);
    }
}

using DropUz.Common.Presentation.Authorization;
using DropUz.Common.Presentation.Endpoints;
using DropUz.Common.Presentation.Results;
using DropUz.Modules.Admin.Application.Settings;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DropUz.Modules.Admin.Presentation;

public sealed class AdminEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/admin/status", () => Results.Ok(new { module = "admin", status = "ok" }))
            .WithTags("Admin")
            .WithName("GetAdminStatus");

        app.MapGet("/api/support/telegram", async (ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetSupportTelegramUrlQuery(), cancellationToken)).ToHttpResult())
            .WithTags("Support")
            .WithName("GetSupportTelegramUrl");

        RouteGroupBuilder admin = app
            .MapGroup("/api/admin/settings")
            .WithTags("Admin Settings")
            .RequireAdmin();

        admin.MapGet("/support-telegram-url", async (ISender sender, CancellationToken cancellationToken) =>
            (await sender.Send(new GetSupportTelegramUrlQuery(), cancellationToken)).ToHttpResult())
            .WithName("GetAdminSupportTelegramUrl");

        admin.MapPut("/support-telegram-url", async (
            SetSupportTelegramUrlRequest request,
            ISender sender,
            CancellationToken cancellationToken) =>
            (await sender.Send(new SetSupportTelegramUrlCommand(request.Url), cancellationToken)).ToHttpResult())
            .WithName("SetSupportTelegramUrl");
    }
}

public sealed record SetSupportTelegramUrlRequest(string Url);

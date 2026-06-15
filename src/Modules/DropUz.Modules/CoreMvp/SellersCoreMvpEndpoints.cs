using DropUz.Common.Application.Abstractions;
using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Modules.CoreMvp;

public sealed class SellersCoreMvpEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/sellers")
            .RequireAuthorization()
            .WithTags("Sellers");

        group.MapPost("/shops", CreateShopAsync);
        group.MapGet("/shops/my", GetMyShopsAsync);
        group.MapPost("/shops/{shopId:guid}/products", AddProductAsync);
    }

    private static async Task<IResult> CreateShopAsync(
        CreateSellerShopRequest request,
        ICurrentUser currentUser,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (!CoreMvpEndpointHelpers.TryGetCurrentUserId(currentUser, out Guid userId))
        {
            return CoreMvpEndpointHelpers.UnauthorizedCurrentUser();
        }

        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Slug))
        {
            return Results.Problem("Shop name and slug are required.", statusCode: StatusCodes.Status400BadRequest);
        }

        string slug = request.Slug.Trim().ToLowerInvariant();
        bool slugExists = await context
            .Set<MvpSellerShop>()
            .AnyAsync(shop => shop.Slug == slug, cancellationToken);

        if (slugExists)
        {
            return Results.Problem("Shop slug is already used.", statusCode: StatusCodes.Status400BadRequest);
        }

        var shop = new MvpSellerShop
        {
            Id = Guid.NewGuid(),
            OwnerUserId = userId,
            Name = request.Name.Trim(),
            Slug = slug,
            GlobalMarkupPercent = request.GlobalMarkupPercent,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        context.Set<MvpSellerShop>().Add(shop);
        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/api/sellers/shops/{shop.Id}", shop.ToResponse());
    }

    private static async Task<IResult> GetMyShopsAsync(
        ICurrentUser currentUser,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (!CoreMvpEndpointHelpers.TryGetCurrentUserId(currentUser, out Guid userId))
        {
            return CoreMvpEndpointHelpers.UnauthorizedCurrentUser();
        }

        SellerShopResponse[] shops = await context
            .Set<MvpSellerShop>()
            .Where(shop => shop.OwnerUserId == userId)
            .OrderBy(shop => shop.Name)
            .Select(shop => shop.ToResponse())
            .ToArrayAsync(cancellationToken);

        return TypedResults.Ok(shops);
    }

    private static async Task<IResult> AddProductAsync(
        Guid shopId,
        AddSellerProductRequest request,
        ICurrentUser currentUser,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (!CoreMvpEndpointHelpers.TryGetCurrentUserId(currentUser, out Guid userId))
        {
            return CoreMvpEndpointHelpers.UnauthorizedCurrentUser();
        }

        MvpSellerShop? shop = await context
            .Set<MvpSellerShop>()
            .FirstOrDefaultAsync(item => item.Id == shopId && item.OwnerUserId == userId, cancellationToken);

        if (shop is null)
        {
            return Results.NotFound();
        }

        MvpProduct? product = await context
            .Set<MvpProduct>()
            .FirstOrDefaultAsync(item => item.Id == request.ProductId && item.IsApproved, cancellationToken);

        if (product is null)
        {
            return Results.Problem("Approved product was not found.", statusCode: StatusCodes.Status400BadRequest);
        }

        bool alreadyExists = await context
            .Set<MvpSellerProduct>()
            .AnyAsync(item => item.SellerShopId == shopId && item.ProductId == request.ProductId, cancellationToken);

        if (alreadyExists)
        {
            return Results.Problem("Product is already attached to this shop.", statusCode: StatusCodes.Status400BadRequest);
        }

        var sellerProduct = new MvpSellerProduct
        {
            Id = Guid.NewGuid(),
            SellerShopId = shopId,
            ProductId = request.ProductId,
            MarkupPercent = request.MarkupPercent,
            SellerPrice = CoreMvpEndpointHelpers.CalculateSellerPrice(product, request.MarkupPercent),
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        context.Set<MvpSellerProduct>().Add(sellerProduct);
        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/api/sellers/shops/{shopId}/products/{sellerProduct.Id}", sellerProduct.ToResponse());
    }
}

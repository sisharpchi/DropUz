using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Modules.CoreMvp;

public sealed class CatalogCoreMvpEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/catalog")
            .WithTags("Catalog");

        group.MapGet("/products", ListProductsAsync)
            .AllowAnonymous();

        group.MapGet("/products/{id:guid}", GetProductAsync)
            .AllowAnonymous();

        group.MapPost("/products", CreateProductAsync)
            .RequireAuthorization();

        group.MapPost("/products/{id:guid}/approve", ApproveProductAsync)
            .RequireAuthorization();
    }

    private static async Task<IResult> ListProductsAsync(
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        ProductResponse[] products = await context
            .Set<MvpProduct>()
            .Where(product => product.IsApproved)
            .OrderByDescending(product => product.CreatedAtUtc)
            .Select(product => product.ToResponse())
            .ToArrayAsync(cancellationToken);

        return TypedResults.Ok(products);
    }

    private static async Task<IResult> GetProductAsync(
        Guid id,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        MvpProduct? product = await context
            .Set<MvpProduct>()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return product is null
            ? Results.NotFound()
            : TypedResults.Ok(product.ToResponse());
    }

    private static async Task<IResult> CreateProductAsync(
        CreateProductRequest request,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.SourcePrice <= 0)
        {
            return Results.Problem("Product name and positive source price are required.", statusCode: StatusCodes.Status400BadRequest);
        }

        var product = new MvpProduct
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            SourceUrl = request.SourceUrl.Trim(),
            SourcePrice = CoreMvpEndpointHelpers.Money(request.SourcePrice),
            DropUzMarkupPercent = request.DropUzMarkupPercent,
            Price = CoreMvpEndpointHelpers.CalculateDropUzPrice(request.SourcePrice, request.DropUzMarkupPercent),
            IsApproved = false,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        context.Set<MvpProduct>().Add(product);
        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/api/catalog/products/{product.Id}", product.ToResponse());
    }

    private static async Task<IResult> ApproveProductAsync(
        Guid id,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        MvpProduct? product = await context
            .Set<MvpProduct>()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (product is null)
        {
            return Results.NotFound();
        }

        product.IsApproved = true;
        await context.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok(product.ToResponse());
    }
}

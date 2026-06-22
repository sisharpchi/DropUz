using DropUz.Common.Application.Clock;
using DropUz.Common.Application.Data;
using DropUz.Common.Application.Messaging;
using DropUz.Common.Domain;
using DropUz.Modules.Catalog.Domain.Categories;
using DropUz.Modules.Catalog.Domain.Pricing;
using DropUz.Modules.Catalog.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Modules.Catalog.Application.Products;

public sealed class CatalogPricingService(IMainRepository repository) : ICatalogPricingService
{
    public async Task<Result<CatalogPriceQuote>> CalculateDropUzPriceAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        CatalogProduct? product = await repository.GetAsync<CatalogProduct>(productId);
        if (product is null)
        {
            return Result.Failure<CatalogPriceQuote>(CatalogErrors.ProductNotFound);
        }

        Markup globalMarkup = await GetGlobalMarkupAsync(repository, cancellationToken);
        PriceBreakdown price = DropUzPriceCalculator.Calculate(
            product.ApiPriceInUzs,
            globalMarkup,
            product.ProductMarkup);

        return Result.Success(new CatalogPriceQuote(
            product.Id,
            product.ApiPrice,
            product.CurrencyRate,
            price.AppliedMarkup,
            price.MarkupAmount,
            price.FinalPrice));
    }

    internal static async Task<Markup> GetGlobalMarkupAsync(
        IMainRepository repository,
        CancellationToken cancellationToken)
    {
        CatalogPricingSettings? settings = await repository
            .Query<CatalogPricingSettings>(x => x.Id == CatalogPricingSettings.DefaultId)
            .FirstOrDefaultAsync(cancellationToken);

        return settings?.GlobalMarkup ?? new Markup(MarkupType.Percent, 0m);
    }
}

public sealed class ImportProductCommandHandler(
    IMainRepository repository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<ImportProductCommand, CatalogProductResponse>
{
    public async Task<Result<CatalogProductResponse>> Handle(
        ImportProductCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
        {
            return Result.Failure<CatalogProductResponse>(CatalogErrors.ProductNameRequired);
        }

        if (string.IsNullOrWhiteSpace(command.SourcePlatform) ||
            string.IsNullOrWhiteSpace(command.SourceProductId))
        {
            return Result.Failure<CatalogProductResponse>(CatalogErrors.SourceProductRequired);
        }

        if (command.ApiPrice <= 0m)
        {
            return Result.Failure<CatalogProductResponse>(CatalogErrors.ApiPriceInvalid);
        }

        if (command.CategoryId.HasValue &&
            !await repository.Query<Category>(category => category.Id == command.CategoryId.Value).AnyAsync(cancellationToken))
        {
            return Result.Failure<CatalogProductResponse>(CatalogErrors.CategoryNotFound);
        }

        CatalogProduct? product = await repository
            .Query<CatalogProduct>(x =>
                x.SourcePlatform == command.SourcePlatform.Trim() &&
                x.SourceProductId == command.SourceProductId.Trim())
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            product = CatalogProduct.Import(
                command.CategoryId,
                command.Name,
                command.Description,
                command.ImageUrl,
                command.SourcePlatform,
                command.SourceProductId,
                command.SourceUrl,
                command.ApiPrice,
                command.CurrencyCode,
                command.CurrencyRate,
                dateTimeProvider.UtcNow);

            await repository.AddAsync(product);
        }
        else
        {
            product.UpdateImportData(
                command.CategoryId,
                command.Name,
                command.Description,
                command.ImageUrl,
                command.ApiPrice,
                command.CurrencyCode,
                command.CurrencyRate,
                dateTimeProvider.UtcNow);
        }

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(await ProductMapper.MapAsync(repository, product, cancellationToken));
    }
}

public sealed class ApproveProductCommandHandler(
    IMainRepository repository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<ApproveProductCommand, CatalogProductResponse>
{
    public async Task<Result<CatalogProductResponse>> Handle(
        ApproveProductCommand command,
        CancellationToken cancellationToken)
    {
        CatalogProduct? product = await repository.GetAsync<CatalogProduct>(command.ProductId);
        if (product is null)
        {
            return Result.Failure<CatalogProductResponse>(CatalogErrors.ProductNotFound);
        }

        product.Approve(dateTimeProvider.UtcNow);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(await ProductMapper.MapAsync(repository, product, cancellationToken));
    }
}

public sealed class RejectProductCommandHandler(
    IMainRepository repository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<RejectProductCommand, CatalogProductResponse>
{
    public async Task<Result<CatalogProductResponse>> Handle(
        RejectProductCommand command,
        CancellationToken cancellationToken)
    {
        CatalogProduct? product = await repository.GetAsync<CatalogProduct>(command.ProductId);
        if (product is null)
        {
            return Result.Failure<CatalogProductResponse>(CatalogErrors.ProductNotFound);
        }

        product.Reject(dateTimeProvider.UtcNow);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(await ProductMapper.MapAsync(repository, product, cancellationToken));
    }
}

public sealed class SetGlobalDropUzMarkupCommandHandler(
    IMainRepository repository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<SetGlobalDropUzMarkupCommand>
{
    public async Task<Result> Handle(
        SetGlobalDropUzMarkupCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Markup.Value < 0m)
        {
            return Result.Failure(CatalogErrors.MarkupInvalid);
        }

        CatalogPricingSettings? settings = await repository
            .Query<CatalogPricingSettings>(x => x.Id == CatalogPricingSettings.DefaultId)
            .FirstOrDefaultAsync(cancellationToken);

        if (settings is null)
        {
            settings = CatalogPricingSettings.CreateDefault(dateTimeProvider.UtcNow);
            await repository.AddAsync(settings);
        }

        settings.SetGlobalMarkup(command.Markup.ToMarkup(), dateTimeProvider.UtcNow);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

public sealed class SetProductDropUzMarkupCommandHandler(
    IMainRepository repository,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<SetProductDropUzMarkupCommand, CatalogProductResponse>
{
    public async Task<Result<CatalogProductResponse>> Handle(
        SetProductDropUzMarkupCommand command,
        CancellationToken cancellationToken)
    {
        if (command.Markup?.Value < 0m)
        {
            return Result.Failure<CatalogProductResponse>(CatalogErrors.MarkupInvalid);
        }

        CatalogProduct? product = await repository.GetAsync<CatalogProduct>(command.ProductId);
        if (product is null)
        {
            return Result.Failure<CatalogProductResponse>(CatalogErrors.ProductNotFound);
        }

        product.SetMarkup(command.Markup?.ToMarkup(), dateTimeProvider.UtcNow);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(await ProductMapper.MapAsync(repository, product, cancellationToken));
    }
}

public sealed class GetCatalogProductsQueryHandler(IMainRepository repository)
    : IQueryHandler<GetCatalogProductsQuery, IReadOnlyCollection<CatalogProductResponse>>
{
    public async Task<Result<IReadOnlyCollection<CatalogProductResponse>>> Handle(
        GetCatalogProductsQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<CatalogProduct> query = repository.Query<CatalogProduct>();

        if (request.ApprovedOnly)
        {
            query = query.Where(product => product.Status == ProductStatus.Approved);
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(product => product.CategoryId == request.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            string search = request.Search.Trim().ToLower();
            query = query.Where(product => product.Name.ToLower().Contains(search));
        }

        CatalogProduct[] products = await query
            .OrderBy(product => product.Name)
            .Take(100)
            .ToArrayAsync(cancellationToken);

        var responses = new List<CatalogProductResponse>(products.Length);
        foreach (CatalogProduct product in products)
        {
            responses.Add(await ProductMapper.MapAsync(repository, product, cancellationToken));
        }

        return Result.Success<IReadOnlyCollection<CatalogProductResponse>>(responses);
    }
}

public sealed class GetCatalogProductQueryHandler(IMainRepository repository)
    : IQueryHandler<GetCatalogProductQuery, CatalogProductResponse>
{
    public async Task<Result<CatalogProductResponse>> Handle(
        GetCatalogProductQuery request,
        CancellationToken cancellationToken)
    {
        CatalogProduct? product = await repository.GetAsync<CatalogProduct>(request.ProductId);
        if (product is null)
        {
            return Result.Failure<CatalogProductResponse>(CatalogErrors.ProductNotFound);
        }

        return Result.Success(await ProductMapper.MapAsync(repository, product, cancellationToken));
    }
}

internal static class ProductMapper
{
    internal static async Task<CatalogProductResponse> MapAsync(
        IMainRepository repository,
        CatalogProduct product,
        CancellationToken cancellationToken)
    {
        Markup globalMarkup = await CatalogPricingService.GetGlobalMarkupAsync(repository, cancellationToken);
        PriceBreakdown price = DropUzPriceCalculator.Calculate(
            product.ApiPriceInUzs,
            globalMarkup,
            product.ProductMarkup);

        return new CatalogProductResponse(
            product.Id,
            product.CategoryId,
            product.Name,
            product.Description,
            product.ImageUrl,
            product.SourcePlatform,
            product.SourceProductId,
            product.SourceUrl,
            product.ApiPrice,
            product.CurrencyCode,
            product.CurrencyRate,
            product.Status,
            product.ProductMarkup,
            price.AppliedMarkup,
            price.MarkupAmount,
            price.FinalPrice);
    }
}

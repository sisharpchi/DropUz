using DropUz.Common.Domain;
using DropUz.Modules.Catalog.Domain.Pricing;

namespace DropUz.Modules.Catalog.Domain.Products;

public sealed class CatalogProduct : Entity
{
    private CatalogProduct()
    {
    }

    private CatalogProduct(
        Guid id,
        Guid? categoryId,
        string name,
        string? description,
        string? imageUrl,
        string sourcePlatform,
        string sourceProductId,
        string? sourceUrl,
        decimal apiPrice,
        string currencyCode,
        decimal currencyRate,
        DateTime createdAtUtc)
        : base(id)
    {
        CategoryId = categoryId;
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        SourcePlatform = sourcePlatform;
        SourceProductId = sourceProductId;
        SourceUrl = sourceUrl;
        ApiPrice = apiPrice;
        CurrencyCode = currencyCode;
        CurrencyRate = currencyRate <= 0m ? 1m : currencyRate;
        Status = ProductStatus.Imported;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public Guid? CategoryId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string? ImageUrl { get; private set; }

    public string SourcePlatform { get; private set; } = string.Empty;

    public string SourceProductId { get; private set; } = string.Empty;

    public string? SourceUrl { get; private set; }

    public decimal ApiPrice { get; private set; }

    public string CurrencyCode { get; private set; } = "UZS";

    public decimal CurrencyRate { get; private set; } = 1m;

    public MarkupType? DropUzMarkupType { get; private set; }

    public decimal? DropUzMarkupValue { get; private set; }

    public ProductStatus Status { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public static CatalogProduct Import(
        Guid? categoryId,
        string name,
        string? description,
        string? imageUrl,
        string sourcePlatform,
        string sourceProductId,
        string? sourceUrl,
        decimal apiPrice,
        string currencyCode,
        decimal currencyRate,
        DateTime createdAtUtc)
    {
        return new CatalogProduct(
            Guid.NewGuid(),
            categoryId,
            name.Trim(),
            description,
            imageUrl,
            sourcePlatform.Trim(),
            sourceProductId.Trim(),
            sourceUrl,
            apiPrice,
            string.IsNullOrWhiteSpace(currencyCode) ? "UZS" : currencyCode.Trim().ToUpperInvariant(),
            currencyRate,
            createdAtUtc);
    }

    public Markup? ProductMarkup => DropUzMarkupType.HasValue && DropUzMarkupValue.HasValue
        ? new Markup(DropUzMarkupType.Value, DropUzMarkupValue.Value)
        : null;

    public decimal ApiPriceInUzs => decimal.Round(ApiPrice * CurrencyRate, 2, MidpointRounding.AwayFromZero);

    public void Approve(DateTime nowUtc)
    {
        Status = ProductStatus.Approved;
        UpdatedAtUtc = nowUtc;
    }

    public void Reject(DateTime nowUtc)
    {
        Status = ProductStatus.Rejected;
        UpdatedAtUtc = nowUtc;
    }

    public void SetMarkup(Markup? markup, DateTime nowUtc)
    {
        DropUzMarkupType = markup?.Type;
        DropUzMarkupValue = markup?.Value;
        UpdatedAtUtc = nowUtc;
    }

    public void UpdateImportData(
        Guid? categoryId,
        string name,
        string? description,
        string? imageUrl,
        decimal apiPrice,
        string currencyCode,
        decimal currencyRate,
        DateTime nowUtc)
    {
        CategoryId = categoryId;
        Name = name.Trim();
        Description = description;
        ImageUrl = imageUrl;
        ApiPrice = apiPrice;
        CurrencyCode = string.IsNullOrWhiteSpace(currencyCode) ? CurrencyCode : currencyCode.Trim().ToUpperInvariant();
        CurrencyRate = currencyRate <= 0m ? CurrencyRate : currencyRate;
        UpdatedAtUtc = nowUtc;
    }
}

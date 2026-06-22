using DropUz.Common.Domain;

namespace DropUz.Modules.Catalog.Domain.Pricing;

public sealed class CatalogPricingSettings : Entity
{
    public static readonly Guid DefaultId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private CatalogPricingSettings()
    {
    }

    private CatalogPricingSettings(Guid id, Markup globalMarkup, DateTime updatedAtUtc)
        : base(id)
    {
        GlobalMarkupType = globalMarkup.Type;
        GlobalMarkupValue = globalMarkup.Value;
        UpdatedAtUtc = updatedAtUtc;
    }

    public MarkupType GlobalMarkupType { get; private set; }

    public decimal GlobalMarkupValue { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public Markup GlobalMarkup => new(GlobalMarkupType, GlobalMarkupValue);

    public static CatalogPricingSettings CreateDefault(DateTime nowUtc)
    {
        return new CatalogPricingSettings(DefaultId, new Markup(MarkupType.Percent, 0m), nowUtc);
    }

    public void SetGlobalMarkup(Markup markup, DateTime nowUtc)
    {
        GlobalMarkupType = markup.Type;
        GlobalMarkupValue = markup.Value;
        UpdatedAtUtc = nowUtc;
    }
}

using DropUz.Modules.Catalog.Domain.Pricing;

namespace DropUz.Modules.Catalog.Application;

public sealed record MarkupInput(MarkupType Type, decimal Value)
{
    public Markup ToMarkup()
    {
        return new Markup(Type, Value);
    }
}

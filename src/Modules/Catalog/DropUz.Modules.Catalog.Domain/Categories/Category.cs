using DropUz.Common.Domain;

namespace DropUz.Modules.Catalog.Domain.Categories;

public sealed class Category : Entity
{
    private Category()
    {
    }

    private Category(Guid id, string name, string slug, DateTime createdAtUtc)
        : base(id)
    {
        Name = name;
        Slug = slug;
        CreatedAtUtc = createdAtUtc;
    }

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public static Category Create(string name, string slug, DateTime createdAtUtc)
    {
        return new Category(Guid.NewGuid(), name.Trim(), slug.Trim().ToLowerInvariant(), createdAtUtc);
    }

    public void Rename(string name, string slug)
    {
        Name = name.Trim();
        Slug = slug.Trim().ToLowerInvariant();
    }
}

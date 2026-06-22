namespace DropUz.Modules.Catalog.Application.Categories;

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string Slug);

using DropUz.Common.Application.Messaging;

namespace DropUz.Modules.Catalog.Application.Categories;

public sealed record CreateCategoryCommand(string Name, string Slug) : ICommand<CategoryResponse>;

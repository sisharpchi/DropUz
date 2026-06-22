using DropUz.Common.Domain;

namespace DropUz.Modules.Catalog.Application;

public static class CatalogErrors
{
    public static readonly Error CategoryNameRequired = Error.Validation(
        "Catalog.CategoryNameRequired",
        "Category name is required.");

    public static readonly Error ProductNameRequired = Error.Validation(
        "Catalog.ProductNameRequired",
        "Product name is required.");

    public static readonly Error SourceProductRequired = Error.Validation(
        "Catalog.SourceProductRequired",
        "Source platform and product id are required.");

    public static readonly Error ApiPriceInvalid = Error.Validation(
        "Catalog.ApiPriceInvalid",
        "API price must be greater than zero.");

    public static readonly Error MarkupInvalid = Error.Validation(
        "Catalog.MarkupInvalid",
        "Markup value cannot be negative.");

    public static readonly Error ProductNotFound = Error.NotFound(
        "Catalog.ProductNotFound",
        "Product was not found.");

    public static readonly Error CategoryNotFound = Error.NotFound(
        "Catalog.CategoryNotFound",
        "Category was not found.");
}

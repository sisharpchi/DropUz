using DropUz.Common.Domain;

namespace DropUz.Modules.Cart.Application;

public static class CartErrors
{
    public static readonly Error UserNotAuthenticated = Error.Unauthorized(
        "Cart.UserNotAuthenticated",
        "Authenticated user is required.");

    public static readonly Error CartNotFound = Error.NotFound(
        "Cart.CartNotFound",
        "Cart was not found.");

    public static readonly Error CartItemNotFound = Error.NotFound(
        "Cart.CartItemNotFound",
        "Cart item was not found.");

    public static readonly Error QuantityInvalid = Error.Validation(
        "Cart.QuantityInvalid",
        "Quantity must be greater than zero.");
}

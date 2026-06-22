using DropUz.Common.Domain;

namespace DropUz.Modules.Payments.Application;

public static class PaymentErrors
{
    public static readonly Error UserNotAuthenticated = Error.Unauthorized(
        "Payments.UserNotAuthenticated",
        "Authenticated user is required.");

    public static readonly Error PaymentNotFound = Error.NotFound(
        "Payments.PaymentNotFound",
        "Payment was not found.");

    public static readonly Error OrderNotFound = Error.NotFound(
        "Payments.OrderNotFound",
        "Order was not found.");

    public static readonly Error PaymentNotAllowed = Error.Validation(
        "Payments.PaymentNotAllowed",
        "Payment is not allowed for the current order status.");
}

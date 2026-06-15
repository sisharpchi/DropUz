using DropUz.Common.Application.Abstractions;
using DropUz.Common.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Modules.CoreMvp;

internal static class CoreMvpEndpointHelpers
{
    public static IResult UnauthorizedCurrentUser()
    {
        return Results.Problem(
            title: "Current user is required.",
            statusCode: StatusCodes.Status401Unauthorized);
    }

    public static bool TryGetCurrentUserId(ICurrentUser currentUser, out Guid userId)
    {
        if (currentUser.UserId is Guid currentUserId)
        {
            userId = currentUserId;
            return true;
        }

        userId = Guid.Empty;
        return false;
    }

    public static decimal CalculateDropUzPrice(decimal sourcePrice, decimal markupPercent)
    {
        return Money(sourcePrice * (1 + markupPercent / 100));
    }

    public static decimal CalculateSellerPrice(MvpProduct product, decimal sellerMarkupPercent)
    {
        return Money(product.SourcePrice * (1 + (product.DropUzMarkupPercent + sellerMarkupPercent) / 100));
    }

    public static decimal Money(decimal amount)
    {
        return decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
    }

    public static ProductResponse ToResponse(this MvpProduct product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.SourceUrl,
            product.SourcePrice,
            product.DropUzMarkupPercent,
            product.Price,
            product.IsApproved);
    }

    public static SellerShopResponse ToResponse(this MvpSellerShop shop)
    {
        return new SellerShopResponse(
            shop.Id,
            shop.Name,
            shop.Slug,
            shop.GlobalMarkupPercent);
    }

    public static SellerProductResponse ToResponse(this MvpSellerProduct sellerProduct)
    {
        return new SellerProductResponse(
            sellerProduct.Id,
            sellerProduct.SellerShopId,
            sellerProduct.ProductId,
            sellerProduct.MarkupPercent,
            sellerProduct.SellerPrice);
    }

    public static CartResponse ToCartResponse(IReadOnlyCollection<MvpCartItem> items)
    {
        CartItemResponse[] responses = items
            .OrderBy(item => item.CreatedAtUtc)
            .Select(item => new CartItemResponse(
                item.Id,
                item.ProductId,
                item.SellerShopId,
                item.ProductName,
                item.UnitPrice,
                item.Quantity,
                Money(item.UnitPrice * item.Quantity)))
            .ToArray();

        return new CartResponse(
            responses,
            Money(responses.Sum(item => item.LineTotal)));
    }

    public static async Task<OrderResponse> ToOrderResponseAsync(
        this MvpOrder order,
        MainDbContext context,
        CancellationToken cancellationToken)
    {
        OrderItemResponse[] items = await context
            .Set<MvpOrderItem>()
            .Where(item => item.OrderId == order.Id)
            .OrderBy(item => item.Id)
            .Select(item => new OrderItemResponse(
                item.Id,
                item.ProductId,
                item.SellerShopId,
                item.ProductName,
                item.UnitPrice,
                item.Quantity,
                item.LineTotal))
            .ToArrayAsync(cancellationToken);

        return new OrderResponse(
            order.Id,
            order.Status,
            order.ProductTotal,
            order.CargoPrice,
            order.CargoPaymentDeadlineUtc,
            items);
    }

    public static NotificationResponse ToResponse(this MvpNotification notification)
    {
        return new NotificationResponse(
            notification.Id,
            notification.OrderId,
            notification.Type,
            notification.Message,
            notification.CreatedAtUtc);
    }

    public static void AddNotification(
        MainDbContext context,
        Guid? userId,
        Guid? orderId,
        string type,
        string message)
    {
        context.Set<MvpNotification>().Add(new MvpNotification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OrderId = orderId,
            Type = type,
            Message = message,
            CreatedAtUtc = DateTimeOffset.UtcNow
        });
    }
}

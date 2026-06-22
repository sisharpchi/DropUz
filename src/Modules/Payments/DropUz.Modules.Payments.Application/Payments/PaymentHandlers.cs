using DropUz.Common.Application.Abstractions;
using DropUz.Common.Application.Clock;
using DropUz.Common.Application.Data;
using DropUz.Common.Application.Messaging;
using DropUz.Common.Domain;
using DropUz.Modules.Notifications.Application.Notifications;
using DropUz.Modules.Notifications.Domain.Notifications;
using DropUz.Modules.Orders.Domain.Orders;
using DropUz.Modules.Payments.Domain.Payments;
using DropUz.Modules.Sellers.Domain.Sellers;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Modules.Payments.Application.Payments;

public sealed class StartPaymentCommandHandler(
    IMainRepository repository,
    ICurrentUser currentUser,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<StartPaymentCommand, PaymentResponse>
{
    public async Task<Result<PaymentResponse>> Handle(
        StartPaymentCommand command,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result.Failure<PaymentResponse>(PaymentErrors.UserNotAuthenticated);
        }

        Order? order = await repository.GetAsync<Order>(command.OrderId);
        if (order is null || order.UserId != currentUser.UserId.Value)
        {
            return Result.Failure<PaymentResponse>(PaymentErrors.OrderNotFound);
        }

        decimal amount = command.Type switch
        {
            PaymentType.ProductPayment when order.Status == OrderStatus.PendingProductPayment => order.ProductTotal,
            PaymentType.CargoPayment when order.Status == OrderStatus.PendingCargoPayment && order.CargoTotal > 0m => order.CargoTotal,
            _ => 0m
        };

        if (amount <= 0m)
        {
            return Result.Failure<PaymentResponse>(PaymentErrors.PaymentNotAllowed);
        }

        Payment payment = Payment.Start(
            order.Id,
            order.UserId,
            command.Type,
            command.Method,
            amount,
            dateTimeProvider.UtcNow);

        await repository.AddAsync(payment);
        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(PaymentMapper.Map(payment));
    }
}

public sealed class ConfirmPaymentCommandHandler(
    IMainRepository repository,
    IDateTimeProvider dateTimeProvider,
    INotificationService notificationService)
    : ICommandHandler<ConfirmPaymentCommand, PaymentResponse>
{
    public async Task<Result<PaymentResponse>> Handle(
        ConfirmPaymentCommand command,
        CancellationToken cancellationToken)
    {
        Payment? payment = await repository.GetAsync<Payment>(command.PaymentId);
        if (payment is null)
        {
            return Result.Failure<PaymentResponse>(PaymentErrors.PaymentNotFound);
        }

        Order? order = await repository.GetAsync<Order>(payment.OrderId);
        if (order is null)
        {
            return Result.Failure<PaymentResponse>(PaymentErrors.OrderNotFound);
        }

        if (payment.Status == PaymentStatus.Paid)
        {
            return Result.Success(PaymentMapper.Map(payment));
        }

        payment.MarkPaid(command.ProviderTransactionId, dateTimeProvider.UtcNow);

        if (payment.Type == PaymentType.ProductPayment)
        {
            order.MarkProductPaid(dateTimeProvider.UtcNow);
            if (order.SellerId.HasValue)
            {
                SellerProfile? seller = await repository.GetAsync<SellerProfile>(order.SellerId.Value);
                seller?.RecordProductPayment(order.Id, order.SellerProfitTotal, dateTimeProvider.UtcNow);
            }
        }
        else
        {
            order.MarkCargoPaid(dateTimeProvider.UtcNow);
        }

        await notificationService.EnqueueAsync(
            order.UserId,
            order.Id,
            NotificationType.PaymentReceived,
            "Payment received",
            $"{payment.Type} payment for order {order.Id} was received.",
            cancellationToken);

        await repository.UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(PaymentMapper.Map(payment));
    }
}

public sealed class GetMyPaymentsQueryHandler(
    IMainRepository repository,
    ICurrentUser currentUser)
    : IQueryHandler<GetMyPaymentsQuery, IReadOnlyCollection<PaymentResponse>>
{
    public async Task<Result<IReadOnlyCollection<PaymentResponse>>> Handle(
        GetMyPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        if (currentUser.UserId is null)
        {
            return Result.Failure<IReadOnlyCollection<PaymentResponse>>(PaymentErrors.UserNotAuthenticated);
        }

        Payment[] payments = await repository
            .Query<Payment>(payment => payment.UserId == currentUser.UserId.Value)
            .OrderByDescending(payment => payment.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<PaymentResponse>>(
            payments.Select(PaymentMapper.Map).ToArray());
    }
}

public sealed class GetAdminPaymentsQueryHandler(IMainRepository repository)
    : IQueryHandler<GetAdminPaymentsQuery, IReadOnlyCollection<PaymentResponse>>
{
    public async Task<Result<IReadOnlyCollection<PaymentResponse>>> Handle(
        GetAdminPaymentsQuery request,
        CancellationToken cancellationToken)
    {
        Payment[] payments = await repository
            .Query<Payment>()
            .OrderByDescending(payment => payment.CreatedAtUtc)
            .Take(200)
            .ToArrayAsync(cancellationToken);

        return Result.Success<IReadOnlyCollection<PaymentResponse>>(
            payments.Select(PaymentMapper.Map).ToArray());
    }
}

internal static class PaymentMapper
{
    internal static PaymentResponse Map(Payment payment)
    {
        return new PaymentResponse(
            payment.Id,
            payment.OrderId,
            payment.UserId,
            payment.Type,
            payment.Method,
            payment.Amount,
            payment.Provider,
            payment.ProviderTransactionId,
            payment.Status,
            payment.CreatedAtUtc,
            payment.PaidAtUtc);
    }
}

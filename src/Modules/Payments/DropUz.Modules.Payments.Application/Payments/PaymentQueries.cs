using DropUz.Common.Application.Messaging;

namespace DropUz.Modules.Payments.Application.Payments;

public sealed record GetMyPaymentsQuery : IQuery<IReadOnlyCollection<PaymentResponse>>;

public sealed record GetAdminPaymentsQuery : IQuery<IReadOnlyCollection<PaymentResponse>>;

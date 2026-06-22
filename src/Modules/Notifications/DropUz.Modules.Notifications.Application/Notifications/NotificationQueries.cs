using DropUz.Common.Application.Messaging;

namespace DropUz.Modules.Notifications.Application.Notifications;

public sealed record GetMyNotificationsQuery : IQuery<IReadOnlyCollection<NotificationResponse>>;

public sealed record GetAdminNotificationsQuery : IQuery<IReadOnlyCollection<NotificationResponse>>;

using DropUz.Common.Application.Messaging;

namespace DropUz.Modules.Notifications.Application.Notifications;

public sealed record LinkTelegramCommand(string ChatId) : ICommand;

public sealed record RetryNotificationCommand(Guid NotificationId) : ICommand<NotificationResponse>;

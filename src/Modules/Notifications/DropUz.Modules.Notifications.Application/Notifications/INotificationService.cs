using DropUz.Modules.Notifications.Domain.Notifications;

namespace DropUz.Modules.Notifications.Application.Notifications;

public interface INotificationService
{
    Task EnqueueAsync(
        Guid userId,
        Guid? orderId,
        NotificationType type,
        string subject,
        string body,
        CancellationToken cancellationToken = default);
}

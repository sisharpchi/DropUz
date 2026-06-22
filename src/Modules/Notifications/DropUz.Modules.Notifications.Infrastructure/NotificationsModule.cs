using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using DropUz.Modules.Notifications.Application.Notifications;
using DropUz.Modules.Notifications.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotificationsApplication = DropUz.Modules.Notifications.Application.AssemblyReference;
using NotificationsPresentation = DropUz.Modules.Notifications.Presentation.AssemblyReference;

namespace DropUz.Modules.Notifications.Infrastructure;

public static class NotificationsModule
{
    public static IServiceCollection AddNotificationsModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(NotificationsApplication.Assembly));

        services.TryAddScoped<INotificationService, NotificationService>();
        services.TryAddEnumerable(ServiceDescriptor.Scoped<IMainDbContextModelContributor, NotificationsModelContributor>());
        services.AddEndpoints(NotificationsPresentation.Assembly);

        return services;
    }
}

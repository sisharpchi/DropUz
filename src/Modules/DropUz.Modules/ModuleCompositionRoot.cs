using DropUz.Common.Presentation.Endpoints;
using DropUz.Modules.Admin.Infrastructure;
using DropUz.Modules.Cargo.Infrastructure;
using DropUz.Modules.Cart.Infrastructure;
using DropUz.Modules.Catalog.Infrastructure;
using DropUz.Modules.Identity.Infrastructure;
using DropUz.Modules.Notifications.Infrastructure;
using DropUz.Modules.Orders.Infrastructure;
using DropUz.Modules.Payments.Infrastructure;
using DropUz.Modules.Sellers.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DropUz.Modules;

public static class ModuleCompositionRoot
{
    public static IServiceCollection AddDropUzModules(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddIdentityModule(configuration);
        services.AddCatalogModule(configuration);
        services.AddSellersModule(configuration);
        services.AddCartModule(configuration);
        services.AddOrdersModule(configuration);
        services.AddPaymentsModule(configuration);
        services.AddCargoModule(configuration);
        services.AddNotificationsModule(configuration);
        services.AddAdminModule(configuration);
        services.AddCoreMvpModule();

        return services;
    }

    public static WebApplication MapDropUzModules(this WebApplication app)
    {
        app.MapEndpoints();

        return app;
    }
}

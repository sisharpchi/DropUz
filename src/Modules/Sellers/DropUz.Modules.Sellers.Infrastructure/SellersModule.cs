using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using DropUz.Modules.Sellers.Application.Sellers;
using DropUz.Modules.Sellers.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SellersApplication = DropUz.Modules.Sellers.Application.AssemblyReference;
using SellersPresentation = DropUz.Modules.Sellers.Presentation.AssemblyReference;

namespace DropUz.Modules.Sellers.Infrastructure;

public static class SellersModule
{
    public static IServiceCollection AddSellersModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(SellersApplication.Assembly));

        services.TryAddScoped<ISellerPricingService, SellerPricingService>();
        services.TryAddEnumerable(ServiceDescriptor.Scoped<IMainDbContextModelContributor, SellersModelContributor>());
        services.AddEndpoints(SellersPresentation.Assembly);

        return services;
    }
}

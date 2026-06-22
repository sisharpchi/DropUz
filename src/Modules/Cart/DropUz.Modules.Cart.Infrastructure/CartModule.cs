using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using DropUz.Modules.Cart.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CartApplication = DropUz.Modules.Cart.Application.AssemblyReference;
using CartPresentation = DropUz.Modules.Cart.Presentation.AssemblyReference;

namespace DropUz.Modules.Cart.Infrastructure;

public static class CartModule
{
    public static IServiceCollection AddCartModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(CartApplication.Assembly));

        services.TryAddEnumerable(ServiceDescriptor.Scoped<IMainDbContextModelContributor, CartModelContributor>());
        services.AddEndpoints(CartPresentation.Assembly);

        return services;
    }
}

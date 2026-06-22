using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using DropUz.Modules.Cargo.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CargoApplication = DropUz.Modules.Cargo.Application.AssemblyReference;
using CargoPresentation = DropUz.Modules.Cargo.Presentation.AssemblyReference;

namespace DropUz.Modules.Cargo.Infrastructure;

public static class CargoModule
{
    public static IServiceCollection AddCargoModule(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(CargoApplication.Assembly));

        services.TryAddEnumerable(ServiceDescriptor.Scoped<IMainDbContextModelContributor, CargoModelContributor>());
        services.AddEndpoints(CargoPresentation.Assembly);

        return services;
    }
}

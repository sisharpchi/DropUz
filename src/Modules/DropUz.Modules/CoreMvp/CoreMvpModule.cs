using System.Reflection;
using DropUz.Common.Infrastructure.Data;
using DropUz.Common.Presentation.Endpoints;
using Microsoft.Extensions.DependencyInjection;

namespace DropUz.Modules;

public static class CoreMvpModule
{
    public static IServiceCollection AddCoreMvpModule(this IServiceCollection services)
    {
        services.AddSingleton<IMainDbContextModelContributor, CoreMvpModelContributor>();
        services.AddEndpoints(Assembly.GetExecutingAssembly());

        return services;
    }
}

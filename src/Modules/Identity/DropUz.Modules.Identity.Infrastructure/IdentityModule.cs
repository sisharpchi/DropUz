using DropUz.Common.Application.Data;
using DropUz.Common.Infrastructure.Persistence;
using DropUz.Common.Presentation.Endpoints;
using DropUz.Modules.Identity.Application.Data;
using DropUz.Modules.Identity.Domain.OpenId;
using DropUz.Modules.Identity.Domain.Roles;
using DropUz.Modules.Identity.Domain.Users;
using DropUz.Modules.Identity.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using IdentityPresentation = DropUz.Modules.Identity.Presentation.AssemblyReference;

namespace DropUz.Modules.Identity.Infrastructure;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services)
    {
        services.AddDbContext<IdentityDbContext>((serviceProvider, options) =>
        {
            string connectionString = serviceProvider
                .GetRequiredService<IDatabaseConnectionStringProvider>()
                .GetConnectionString();

            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(
                    HistoryRepository.DefaultTableName,
                    Schemas.Identity));
        });

        services.AddAuthentication();
        services.AddAuthorization();

        services
            .AddIdentityCore<User>()
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        services
            .AddOpenIddict()
            .AddCore(options =>
            {
                options
                    .UseEntityFrameworkCore()
                    .UseDbContext<IdentityDbContext>()
                    .ReplaceDefaultEntities<
                        OpenIdApplication,
                        OpenIdAuthorization,
                        OpenIdScope,
                        OpenIdToken,
                        long>();
            });

        services.AddScoped<UnitOfWork<IdentityDbContext>>();
        services.AddScoped<IIdentityUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<IdentityDbContext>());
        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddEndpoints(IdentityPresentation.Assembly);

        return services;
    }
}

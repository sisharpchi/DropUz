using DropUz.Modules.Identity.Application.Data;
using DropUz.Modules.Identity.Domain.OpenId;
using DropUz.Modules.Identity.Domain.Roles;
using DropUz.Modules.Identity.Domain.Users;
using DropUz.Modules.Identity.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;

namespace DropUz.Modules.Identity.Infrastructure.Database;

public sealed class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityDbContext<User, AppRole,Guid,
        IdentityUserClaim<Guid>, IdentityUserRole<Guid>, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options), IIdentityUnitOfWork
{
    public DbSet<OpenIdApplication> OpenIdApplications => Set<OpenIdApplication>();
    public DbSet<OpenIdAuthorization> OpenIdAuthorizations => Set<OpenIdAuthorization>();
    public DbSet<OpenIdScope> OpenIdScopes => Set<OpenIdScope>();
    public DbSet<OpenIdToken> OpenIdTokens => Set<OpenIdToken>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema(Schemas.Identity);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), type => type.Namespace?.Contains(".Configurations") == true);
    }
}

using DropUz.Modules.Identity.Domain.OpenId;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DropUz.Modules.Identity.Infrastructure.Configurations;

internal sealed class OpenIdApplicationConfiguration :
    IEntityTypeConfiguration<OpenIdApplication>,
    IEntityTypeConfiguration<OpenIdAuthorization>,
    IEntityTypeConfiguration<OpenIdScope>,
    IEntityTypeConfiguration<OpenIdToken>
{
    public void Configure(EntityTypeBuilder<OpenIdApplication> builder)
    {
        builder.ToTable("openiddict_applications");
    }

    public void Configure(EntityTypeBuilder<OpenIdAuthorization> builder)
    {
        builder.ToTable("openiddict_authorizations");
    }

    public void Configure(EntityTypeBuilder<OpenIdScope> builder)
    {
        builder.ToTable("openiddict_scopes");
    }

    public void Configure(EntityTypeBuilder<OpenIdToken> builder)
    {
        builder.ToTable("openiddict_tokens");
    }
}

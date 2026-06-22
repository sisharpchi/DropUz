using DropUz.Common.Infrastructure.Data;
using DropUz.Modules.Admin.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DropUz.Modules.Admin.Infrastructure.Database;

internal sealed class AdminModelContributor : IMainDbContextModelContributor
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AdminSettingConfiguration());
    }
}

internal sealed class AdminSettingConfiguration : IEntityTypeConfiguration<AdminSetting>
{
    public void Configure(EntityTypeBuilder<AdminSetting> builder)
    {
        builder.ToTable("settings", Schemas.Admin);
        builder.HasKey(setting => setting.Id);
        builder.Property(setting => setting.Key).HasMaxLength(200).IsRequired();
        builder.Property(setting => setting.Value).HasMaxLength(2000).IsRequired();
        builder.HasIndex(setting => setting.Key).IsUnique();
    }
}

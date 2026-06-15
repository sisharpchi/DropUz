using DropUz.Common.Infrastructure.Data;
using DropUz.Modules.CoreMvp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DropUz.Modules;

internal sealed class CoreMvpModelContributor : IMainDbContextModelContributor
{
    public void Configure(ModelBuilder modelBuilder)
    {
        ConfigureProducts(modelBuilder.Entity<MvpProduct>());
        ConfigureSellerShops(modelBuilder.Entity<MvpSellerShop>());
        ConfigureSellerProducts(modelBuilder.Entity<MvpSellerProduct>());
        ConfigureCartItems(modelBuilder.Entity<MvpCartItem>());
        ConfigureOrders(modelBuilder.Entity<MvpOrder>());
        ConfigureOrderItems(modelBuilder.Entity<MvpOrderItem>());
        ConfigurePayments(modelBuilder.Entity<MvpPayment>());
        ConfigureNotifications(modelBuilder.Entity<MvpNotification>());
    }

    private static void ConfigureProducts(EntityTypeBuilder<MvpProduct> builder)
    {
        builder.ToTable("products", "catalog");
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Name).HasMaxLength(200);
        builder.Property(product => product.Description).HasMaxLength(1000);
        builder.Property(product => product.SourceUrl).HasMaxLength(1000);
        builder.Property(product => product.SourcePrice).HasPrecision(18, 2);
        builder.Property(product => product.DropUzMarkupPercent).HasPrecision(9, 2);
        builder.Property(product => product.Price).HasPrecision(18, 2);
    }

    private static void ConfigureSellerShops(EntityTypeBuilder<MvpSellerShop> builder)
    {
        builder.ToTable("shops", "sellers");
        builder.HasKey(shop => shop.Id);
        builder.HasIndex(shop => shop.Slug).IsUnique();
        builder.Property(shop => shop.Name).HasMaxLength(200);
        builder.Property(shop => shop.Slug).HasMaxLength(120);
        builder.Property(shop => shop.GlobalMarkupPercent).HasPrecision(9, 2);
    }

    private static void ConfigureSellerProducts(EntityTypeBuilder<MvpSellerProduct> builder)
    {
        builder.ToTable("seller_products", "sellers");
        builder.HasKey(product => product.Id);
        builder.HasIndex(product => new { product.SellerShopId, product.ProductId }).IsUnique();
        builder.Property(product => product.MarkupPercent).HasPrecision(9, 2);
        builder.Property(product => product.SellerPrice).HasPrecision(18, 2);
    }

    private static void ConfigureCartItems(EntityTypeBuilder<MvpCartItem> builder)
    {
        builder.ToTable("cart_items", "cart");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.ProductName).HasMaxLength(200);
        builder.Property(item => item.UnitPrice).HasPrecision(18, 2);
    }

    private static void ConfigureOrders(EntityTypeBuilder<MvpOrder> builder)
    {
        builder.ToTable("orders", "orders");
        builder.HasKey(order => order.Id);
        builder.Property(order => order.Status).HasMaxLength(80);
        builder.Property(order => order.ProductTotal).HasPrecision(18, 2);
        builder.Property(order => order.CargoPrice).HasPrecision(18, 2);
    }

    private static void ConfigureOrderItems(EntityTypeBuilder<MvpOrderItem> builder)
    {
        builder.ToTable("order_items", "orders");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.ProductName).HasMaxLength(200);
        builder.Property(item => item.UnitPrice).HasPrecision(18, 2);
        builder.Property(item => item.LineTotal).HasPrecision(18, 2);
    }

    private static void ConfigurePayments(EntityTypeBuilder<MvpPayment> builder)
    {
        builder.ToTable("payments", "payments");
        builder.HasKey(payment => payment.Id);
        builder.Property(payment => payment.Kind).HasMaxLength(40);
        builder.Property(payment => payment.Status).HasMaxLength(40);
        builder.Property(payment => payment.Amount).HasPrecision(18, 2);
    }

    private static void ConfigureNotifications(EntityTypeBuilder<MvpNotification> builder)
    {
        builder.ToTable("notifications", "notifications");
        builder.HasKey(notification => notification.Id);
        builder.Property(notification => notification.Type).HasMaxLength(80);
        builder.Property(notification => notification.Message).HasMaxLength(1000);
    }
}

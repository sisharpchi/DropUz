namespace DropUz.Modules.CoreMvp;

public sealed class MvpProduct
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string SourceUrl { get; set; } = string.Empty;

    public decimal SourcePrice { get; set; }

    public decimal DropUzMarkupPercent { get; set; }

    public decimal Price { get; set; }

    public bool IsApproved { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}

public sealed class MvpSellerShop
{
    public Guid Id { get; set; }

    public Guid OwnerUserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public decimal GlobalMarkupPercent { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}

public sealed class MvpSellerProduct
{
    public Guid Id { get; set; }

    public Guid SellerShopId { get; set; }

    public Guid ProductId { get; set; }

    public decimal MarkupPercent { get; set; }

    public decimal SellerPrice { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}

public sealed class MvpCartItem
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? SellerShopId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}

public sealed class MvpOrder
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Status { get; set; } = OrderStatuses.PendingProductPayment;

    public decimal ProductTotal { get; set; }

    public decimal? CargoPrice { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? ProductPaidAtUtc { get; set; }

    public DateTimeOffset? CargoPaymentDeadlineUtc { get; set; }

    public DateTimeOffset? CargoPaidAtUtc { get; set; }

    public DateTimeOffset? DeliveredAtUtc { get; set; }
}

public sealed class MvpOrderItem
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public Guid ProductId { get; set; }

    public Guid? SellerShopId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public decimal LineTotal { get; set; }
}

public sealed class MvpPayment
{
    public Guid Id { get; set; }

    public Guid OrderId { get; set; }

    public string Kind { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public string Status { get; set; } = PaymentStatuses.Paid;

    public DateTimeOffset PaidAtUtc { get; set; }
}

public sealed class MvpNotification
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public Guid? OrderId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; set; }
}

public static class OrderStatuses
{
    public const string PendingProductPayment = "PendingProductPayment";
    public const string ProductPaid = "ProductPaid";
    public const string PendingCargoPayment = "PendingCargoPayment";
    public const string CargoPaid = "CargoPaid";
    public const string Delivered = "Delivered";
}

public static class PaymentKinds
{
    public const string Product = "Product";
    public const string Cargo = "Cargo";
}

public static class PaymentStatuses
{
    public const string Paid = "Paid";
}

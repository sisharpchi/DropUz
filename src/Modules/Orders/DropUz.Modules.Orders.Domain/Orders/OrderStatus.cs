namespace DropUz.Modules.Orders.Domain.Orders;

public enum OrderStatus
{
    PendingProductPayment = 1,
    ProductPaid = 2,
    Purchasing = 3,
    Purchased = 4,
    InForeignWarehouse = 5,
    CargoCalculating = 6,
    PendingCargoPayment = 7,
    CargoPaid = 8,
    InTransit = 9,
    ArrivedUzbekistan = 10,
    Delivered = 11,
    Cancelled = 12,
    Refunded = 13,
    CargoPaymentExpired = 14
}

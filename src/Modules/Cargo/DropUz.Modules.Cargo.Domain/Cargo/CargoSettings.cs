using DropUz.Common.Domain;

namespace DropUz.Modules.Cargo.Domain.Cargo;

public sealed class CargoSettings : Entity
{
    public static readonly Guid DefaultId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    private CargoSettings()
    {
    }

    private CargoSettings(Guid id, int paymentDeadlineDays, DateTime updatedAtUtc)
        : base(id)
    {
        PaymentDeadlineDays = paymentDeadlineDays;
        UpdatedAtUtc = updatedAtUtc;
    }

    public int PaymentDeadlineDays { get; private set; }

    public DateTime UpdatedAtUtc { get; private set; }

    public static CargoSettings CreateDefault(DateTime nowUtc)
    {
        return new CargoSettings(DefaultId, 7, nowUtc);
    }

    public void SetDeadlineDays(int days, DateTime nowUtc)
    {
        PaymentDeadlineDays = days <= 0 ? 7 : days;
        UpdatedAtUtc = nowUtc;
    }
}

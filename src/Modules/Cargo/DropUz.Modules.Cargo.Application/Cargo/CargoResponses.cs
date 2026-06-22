namespace DropUz.Modules.Cargo.Application.Cargo;

public sealed record CargoSettingsResponse(int PaymentDeadlineDays, DateTime UpdatedAtUtc);

public sealed record CargoPriceResponse(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    DateTime DeadlineAtUtc,
    DateTime CreatedAtUtc);

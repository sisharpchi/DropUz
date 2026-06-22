using DropUz.Common.Application.Messaging;

namespace DropUz.Modules.Cargo.Application.Cargo;

public sealed record SetCargoDeadlineSettingsCommand(int DeadlineDays) : ICommand<CargoSettingsResponse>;

public sealed record RecordCargoPriceCommand(Guid OrderId, decimal CargoPrice, int? DeadlineDays) : ICommand<CargoPriceResponse>;

public sealed record ExpireCargoPaymentsCommand : ICommand<int>;

public sealed record SendCargoPaymentRemindersCommand : ICommand<int>;

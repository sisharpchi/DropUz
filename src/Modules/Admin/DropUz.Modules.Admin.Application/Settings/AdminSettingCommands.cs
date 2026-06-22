using DropUz.Common.Application.Messaging;

namespace DropUz.Modules.Admin.Application.Settings;

public sealed record SetSupportTelegramUrlCommand(string Url) : ICommand<SupportTelegramUrlResponse>;

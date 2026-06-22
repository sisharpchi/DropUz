using DropUz.Common.Application.Messaging;

namespace DropUz.Modules.Admin.Application.Settings;

public sealed record GetSupportTelegramUrlQuery : IQuery<SupportTelegramUrlResponse>;

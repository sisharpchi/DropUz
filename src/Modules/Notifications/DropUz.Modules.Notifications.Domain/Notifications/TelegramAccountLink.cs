using DropUz.Common.Domain;

namespace DropUz.Modules.Notifications.Domain.Notifications;

public sealed class TelegramAccountLink : Entity
{
    private TelegramAccountLink()
    {
    }

    private TelegramAccountLink(Guid id, Guid userId, string chatId, DateTime linkedAtUtc)
        : base(id)
    {
        UserId = userId;
        ChatId = chatId;
        LinkedAtUtc = linkedAtUtc;
    }

    public Guid UserId { get; private set; }

    public string ChatId { get; private set; } = string.Empty;

    public DateTime LinkedAtUtc { get; private set; }

    public static TelegramAccountLink Create(Guid userId, string chatId, DateTime linkedAtUtc)
    {
        return new TelegramAccountLink(Guid.NewGuid(), userId, chatId.Trim(), linkedAtUtc);
    }

    public void Relink(string chatId, DateTime linkedAtUtc)
    {
        ChatId = chatId.Trim();
        LinkedAtUtc = linkedAtUtc;
    }
}

using DropUz.Common.Domain;

namespace DropUz.Modules.Admin.Domain.Settings;

public sealed class AdminSetting : Entity
{
    private AdminSetting()
    {
    }

    private AdminSetting(Guid id, string key, string value, DateTime updatedAtUtc)
        : base(id)
    {
        Key = key;
        Value = value;
        UpdatedAtUtc = updatedAtUtc;
    }

    public string Key { get; private set; } = string.Empty;

    public string Value { get; private set; } = string.Empty;

    public DateTime UpdatedAtUtc { get; private set; }

    public static AdminSetting Create(string key, string value, DateTime updatedAtUtc)
    {
        return new AdminSetting(Guid.NewGuid(), key, value, updatedAtUtc);
    }

    public void SetValue(string value, DateTime updatedAtUtc)
    {
        Value = value;
        UpdatedAtUtc = updatedAtUtc;
    }
}

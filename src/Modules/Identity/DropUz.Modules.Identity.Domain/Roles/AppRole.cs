using Microsoft.AspNetCore.Identity;

namespace DropUz.Modules.Identity.Domain.Roles;

public class AppRole : IdentityRole<Guid>
{
    public AppRole()
    {
    }

    public AppRole(string name) : base(name)
    {
        if (!name.StartsWith("app."))
            base.Name = $"app.{name}";

        DisplayName = base.Name;
    }
    public string? DisplayName { get; set; }
}

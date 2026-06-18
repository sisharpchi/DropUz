using OpenIddict.EntityFrameworkCore.Models;

namespace DropUz.Modules.Identity.Domain.OpenId;

public class OpenIdApplication : OpenIddictEntityFrameworkCoreApplication<long, OpenIdAuthorization, OpenIdToken>
{
}

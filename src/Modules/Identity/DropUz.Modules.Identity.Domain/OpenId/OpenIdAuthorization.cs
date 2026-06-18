using OpenIddict.EntityFrameworkCore.Models;

namespace DropUz.Modules.Identity.Domain.OpenId;

public class OpenIdAuthorization : OpenIddictEntityFrameworkCoreAuthorization<long, OpenIdApplication, OpenIdToken>
{
}

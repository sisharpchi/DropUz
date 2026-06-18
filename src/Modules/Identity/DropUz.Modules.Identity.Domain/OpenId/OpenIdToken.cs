using OpenIddict.EntityFrameworkCore.Models;

namespace DropUz.Modules.Identity.Domain.OpenId;

public class OpenIdToken : OpenIddictEntityFrameworkCoreToken<long, OpenIdApplication, OpenIdAuthorization>
{
}

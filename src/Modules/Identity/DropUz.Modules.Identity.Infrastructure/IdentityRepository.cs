using DropUz.Common.Infrastructure.Persistence;
using DropUz.Modules.Identity.Application.Data;
using DropUz.Modules.Identity.Infrastructure.Database;

namespace DropUz.Modules.Identity.Infrastructure;

internal sealed class IdentityRepository(
    IdentityDbContext context,
    UnitOfWork<IdentityDbContext> unitOfWork)
    : GenericRepository<IdentityDbContext>(context, unitOfWork), IIdentityRepository;

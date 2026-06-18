using DropUz.Common.Application.Data;
using DropUz.Common.Infrastructure.Data;

namespace DropUz.Common.Infrastructure.Persistence;

internal class MainRepository(MainDbContext context, UnitOfWork<MainDbContext> unitOfWork)
    : GenericRepository<MainDbContext>(context, unitOfWork), IMainRepository
{
}


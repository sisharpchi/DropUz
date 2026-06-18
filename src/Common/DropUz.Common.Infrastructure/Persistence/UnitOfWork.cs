using DropUz.Common.Application.Data;
using Microsoft.EntityFrameworkCore;

namespace DropUz.Common.Infrastructure.Persistence;

public class UnitOfWork<TContext>(TContext context) : IUnitOfWork
    where TContext : DbContext
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}

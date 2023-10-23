using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public abstract class BaseRepository<T, TId> : IRepository<T, TId>
    where T : BaseEntity<TId>
{
    protected readonly EStoreContext _context;

    protected readonly DbSet<T> _dbSet;

    protected readonly IQueryable<T> _query;

    public BaseRepository(EStoreContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = _context.Set<T>();
        _query = _dbSet.AsQueryable();
    }

    public virtual async Task<T?> GetByIdAsync(TId id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual T Add(T entity)
    {
        _dbSet.Add(entity);
        return entity;
    }

    public virtual T Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;

        return entity;
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}

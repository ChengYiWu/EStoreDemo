namespace Domain.Common;

public interface IRepository<T,TId> 
    where T : BaseEntity<TId> 
{
    Task<T?> GetByIdAsync(TId id);

    T Add(T entity);

    T Update(T entity);

    void Delete(T entity);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

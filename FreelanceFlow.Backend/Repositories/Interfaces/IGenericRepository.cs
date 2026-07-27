using System.Linq.Expressions;

namespace FreelanceFlow.Backend.Repositories.Interfaces;

/// <summary>
/// Basic CRUD shared by every repository. Feature-specific repositories
/// (IClientRepository, IInvoiceRepository, ...) extend this with the
/// queries that are unique to that aggregate.
/// </summary>
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<int> SaveChangesAsync();
}
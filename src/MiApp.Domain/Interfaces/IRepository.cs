namespace MiApp.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T?> GetByIdAsync(object id, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
}

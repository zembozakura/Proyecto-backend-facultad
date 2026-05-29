using MiApp.Domain.Entities;

namespace MiApp.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdWithCategoryAsync(Guid id, CancellationToken ct = default);
    Task<IList<Product>> GetAllWithCategoryAsync(CancellationToken ct = default);
    Task<IList<Product>> GetActivesAsync(CancellationToken ct = default);
}

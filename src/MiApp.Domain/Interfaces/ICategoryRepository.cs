using MiApp.Domain.Entities;

namespace MiApp.Domain.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}

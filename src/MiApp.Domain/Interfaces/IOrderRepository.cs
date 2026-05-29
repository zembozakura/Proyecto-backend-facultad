using MiApp.Domain.Entities;

namespace MiApp.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetWithItemsAsync(Guid id, CancellationToken ct = default);
    Task<IList<Order>> GetAllWithItemsAsync(CancellationToken ct = default);
}

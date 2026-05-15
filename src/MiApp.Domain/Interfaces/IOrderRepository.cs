using MiApp.Domain.Entities;

namespace MiApp.Domain.Interfaces;

/// <summary>
/// Repositorio especializado para Orders con operaciones avanzadas
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Obtiene una orden con todos sus items (eager loading)
    /// </summary>
    Task<Order?> GetOrderWithItemsAsync(Guid orderId, CancellationToken ct = default);

    /// <summary>
    /// Obtiene órdenes activas (no canceladas) con paginación
    /// </summary>
    Task<(IList<Order> Orders, int Total)> GetActiveOrdersAsync(
        int pageNumber, 
        int pageSize, 
        CancellationToken ct = default);

    /// <summary>
    /// Crea una orden con sus items en una transacción atómica
    /// </summary>
    Task CreateOrderWithItemsAsync(
        Order order,
        IEnumerable<OrderItem> items,
        CancellationToken ct = default);
}

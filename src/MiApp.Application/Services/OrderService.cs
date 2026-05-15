using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.Services;

/// <summary>
/// Servicio de aplicación para operaciones CRUD de órdenes
/// Aquí va la lógica de negocio, no en el controlador
/// </summary>
public class OrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    /// <summary>
    /// Obtiene todos los órdenes
    /// </summary>
    public async Task<IList<Order>> GetAllOrdersAsync(CancellationToken ct = default)
    {
        return await _orderRepository.GetAllAsync(ct);
    }

    /// <summary>
    /// Obtiene una orden por ID con sus items
    /// </summary>
    public async Task<Order?> GetOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        return await _orderRepository.GetOrderWithItemsAsync(orderId, ct);
    }

    /// <summary>
    /// Obtiene órdenes activas con paginación
    /// </summary>
    public async Task<(IList<Order> Orders, int Total)> GetActiveOrdersAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        return await _orderRepository.GetActiveOrdersAsync(pageNumber, pageSize, ct);
    }

    /// <summary>
    /// Crea una nueva orden con sus items
    /// La lógica de negocio va aquí: validaciones, cálculos, etc
    /// </summary>
    public async Task<Order> CreateOrderAsync(
        string orderNumber,
        IEnumerable<OrderItem> items,
        CancellationToken ct = default)
    {
        // Lógica de negocio: validaciones
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new ArgumentException("El número de orden es requerido");

        var itemsList = items.ToList();
        if (!itemsList.Any())
            throw new InvalidOperationException("Una orden debe tener al menos un item");

        // Calcular el total
        var totalAmount = itemsList.Sum(i => i.Total);

        // Crear la orden
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        // Guardar en transacción atómica
        await _orderRepository.CreateOrderWithItemsAsync(order, itemsList, ct);

        return order;
    }

    /// <summary>
    /// Cancela una orden (puede tener lógica de negocio más compleja)
    /// </summary>
    public async Task<Order> CancelOrderAsync(Guid orderId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, ct);
        if (order == null)
            throw new KeyNotFoundException($"Orden {orderId} no encontrada");

        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
            throw new InvalidOperationException(
                $"No se puede cancelar una orden con estado {order.Status}");

        order.Status = OrderStatus.Cancelled;
        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(ct);

        return order;
    }
}

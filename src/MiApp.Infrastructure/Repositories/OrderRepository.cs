using Microsoft.EntityFrameworkCore;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using MiApp.Infrastructure.Data;

namespace MiApp.Infrastructure.Repositories;

/// <summary>
/// Repositorio específico para Orders con operaciones avanzadas y transacciones
/// </summary>
public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Obtiene una orden con todos sus items — EAGER LOADING con Include
    /// </summary>
    public async Task<Order?> GetOrderWithItemsAsync(Guid orderId, CancellationToken ct = default)
    {
        return await _context.Orders
            .AsNoTracking()                    // No vamos a modificarla
            .Include(o => o.Items)             // Eager loading: trae los items en el mismo query
                .ThenInclude(oi => oi.Product) // Trae también el producto de cada item
            .FirstOrDefaultAsync(o => o.Id == orderId, ct);
    }

    /// <summary>
    /// Obtiene órdenes activas con paginación — FILTRO + ORDEN + PAGINACIÓN
    /// </summary>
    public async Task<(IList<Order> Orders, int Total)> GetActiveOrdersAsync(
        int pageNumber, 
        int pageSize, 
        CancellationToken ct = default)
    {
        // PASO 1: Contar el total ANTES de paginar (para saber cuántas páginas hay)
        var total = await _context.Orders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .CountAsync(ct);

        // PASO 2: Aplicar filtro, orden y paginación
        var orders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.Status != OrderStatus.Cancelled)    // Filtro: solo activas
            .OrderByDescending(o => o.CreatedAt)              // Orden: más recientes primero
            .Skip((pageNumber - 1) * pageSize)                // Omitir páginas anteriores
            .Take(pageSize)                                    // Tomar solo la página actual
            .ToListAsync(ct);

        return (orders, total);
    }

    /// <summary>
    /// Crea una orden con sus items en UNA SOLA TRANSACCIÓN atómica
    /// Si algo falla, todo se revierte
    /// </summary>
    public async Task CreateOrderWithItemsAsync(
        Order order,
        IEnumerable<OrderItem> items,
        CancellationToken ct = default)
    {
        // Iniciar transacción explícita
        using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            // Agregar la orden primero
            await _context.Orders.AddAsync(order, ct);
            await _context.SaveChangesAsync(ct);

            // Agregar los items (ahora la orden ya tiene ID)
            foreach (var item in items)
            {
                item.OrderId = order.Id;
                await _context.OrderItems.AddAsync(item, ct);
            }

            await _context.SaveChangesAsync(ct);

            // Si todo salió bien, confirmar la transacción
            await transaction.CommitAsync(ct);
        }
        catch
        {
            // Si algo falló, deshacer TODO
            await transaction.RollbackAsync(ct);
            throw;  // Relanzar la excepción para que el Use Case la maneje
        }
    }
}

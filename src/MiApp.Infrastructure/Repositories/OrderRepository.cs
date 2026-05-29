using Microsoft.EntityFrameworkCore;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using MiApp.Infrastructure.Data;

namespace MiApp.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Order?> GetWithItemsAsync(Guid id, CancellationToken ct = default) =>
        await _context.Orders
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    public async Task<IList<Order>> GetAllWithItemsAsync(CancellationToken ct = default) =>
        await _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
                .ThenInclude(oi => oi.Product)
            .ToListAsync(ct);
}

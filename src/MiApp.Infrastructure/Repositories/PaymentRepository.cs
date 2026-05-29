using Microsoft.EntityFrameworkCore;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using MiApp.Infrastructure.Data;

namespace MiApp.Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IList<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default) =>
        await _context.Payments
            .AsNoTracking()
            .Where(p => p.OrderId == orderId)
            .ToListAsync(ct);
}

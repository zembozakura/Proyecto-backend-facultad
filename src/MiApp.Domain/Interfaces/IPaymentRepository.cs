using MiApp.Domain.Entities;

namespace MiApp.Domain.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IList<Payment>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default);
}

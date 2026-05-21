using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using MiApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiApp.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio de Pagos
    /// </summary>
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtener pagos por orderId
        /// </summary>
        public async Task<List<Payment>> GetByOrderIdAsync(Guid orderId)
        {
            return await _dbSet
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Obtener pago por Mercado Pago ID
        /// </summary>
        public async Task<Payment?> GetByMercadoPagoIdAsync(string mercadoPagoId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.MercadoPagoId == mercadoPagoId);
        }

        /// <summary>
        /// Obtener pagos por estado
        /// </summary>
        public async Task<List<Payment>> GetByStatusAsync(PaymentStatus status)
        {
            return await _dbSet
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Actualizar estado de pago
        /// </summary>
        public async Task<Payment?> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status)
        {
            var payment = await _dbSet.FindAsync(paymentId);
            if (payment == null)
                return null;

            payment.Status = status;
            payment.UpdatedAt = System.DateTime.UtcNow;

            Update(payment);
            await SaveChangesAsync();

            return payment;
        }
    }
}

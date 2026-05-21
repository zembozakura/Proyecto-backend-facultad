using MiApp.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiApp.Domain.Interfaces
{
    /// <summary>
    /// Interfaz para operaciones de pago
    /// </summary>
    public interface IPaymentRepository : IRepository<Payment>
    {
        /// <summary>
        /// Obtener pagos por orderId
        /// </summary>
        Task<List<Payment>> GetByOrderIdAsync(Guid orderId);

        /// <summary>
        /// Obtener pago por Mercado Pago ID
        /// </summary>
        Task<Payment?> GetByMercadoPagoIdAsync(string mercadoPagoId);

        /// <summary>
        /// Obtener pagos por estado
        /// </summary>
        Task<List<Payment>> GetByStatusAsync(PaymentStatus status);

        /// <summary>
        /// Actualizar estado de pago
        /// </summary>
        Task<Payment?> UpdatePaymentStatusAsync(int paymentId, PaymentStatus status);
    }
}

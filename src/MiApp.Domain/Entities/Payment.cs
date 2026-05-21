using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace MiApp.Domain.Entities
{
    /// <summary>
    /// Entidad de Pago - Contiene información de los pagos realizados
    /// </summary>
    public class Payment
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; } = "ARS";
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public PaymentMethod Method { get; set; }
        
        // Campos específicos para cada método de pago
        public string? MercadoPagoId { get; set; }
        public string? MercadoPagoPreferenceId { get; set; }
        public string? BankTransferId { get; set; }
        public string? UalaTransactionId { get; set; }
        
        // Auditoría
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Relación
        public Order? Order { get; set; }
    }

    /// <summary>
    /// Estado del Pago
    /// </summary>
    public enum PaymentStatus
    {
        Pending = 0,      // Pendiente
        Processing = 1,   // Procesando
        Completed = 2,    // Completado
        Failed = 3,       // Fallido
        Cancelled = 4,    // Cancelado
        Refunded = 5      // Reembolsado
    }

    /// <summary>
    /// Método de Pago disponible
    /// </summary>
    public enum PaymentMethod
    {
        CreditCard = 0,    // Tarjeta de Crédito
        DebitCard = 1,     // Tarjeta de Débito
        MercadoPago = 2,   // Mercado Pago (Billetera)
        BankTransfer = 3,  // Transferencia Bancaria
        Uala = 4,          // Uala (Billetera Digital)
        Cash = 5           // Efectivo/Otro
    }
}

using MiApp.Domain.Entities;
using System;

namespace MiApp.Application.DTOs
{
    /// <summary>
    /// DTO para crear un pago
    /// </summary>
    public class CreatePaymentDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de Mercado Pago
    /// </summary>
    public class MercadoPagoPreferenceDto
    {
        public string? Id { get; set; }
        public string? PreferenceId { get; set; }
        public string? InitPoint { get; set; }
        public string? SandboxInitPoint { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// DTO para respuesta de webhook de Mercado Pago
    /// </summary>
    public class MercadoPagoWebhookDto
    {
        public string? Type { get; set; }
        public Data? Data { get; set; }
    }

    public class Data
    {
        public string? Id { get; set; }
    }

    /// <summary>
    /// DTO para información de pago
    /// </summary>
    public class PaymentDto
    {
        public int Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public PaymentStatus Status { get; set; }
        public PaymentMethod Method { get; set; }
        public string? MercadoPagoId { get; set; }
        public string? BankTransferId { get; set; }
        public string? UalaTransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO para crear pago con transferencia bancaria
    /// </summary>
    public class CreateBankTransferDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? BankName { get; set; }
        public string? Reference { get; set; }
    }

    /// <summary>
    /// DTO para crear pago con Uala
    /// </summary>
    public class CreateUalaPaymentDto
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? PhoneNumber { get; set; }
    }
}

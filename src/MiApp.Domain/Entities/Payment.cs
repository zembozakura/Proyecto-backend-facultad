using MiApp.Domain.Common;

namespace MiApp.Domain.Entities;

public enum PaymentStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4,
    Refunded = 5
}

public enum PaymentMethod
{
    CreditCard = 0,
    DebitCard = 1,
    MercadoPago = 2,
    BankTransfer = 3,
    Uala = 4,
    Cash = 5
}

public class Payment : BaseEntity
{
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "ARS";
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public PaymentMethod Method { get; private set; }
    public string? MercadoPagoId { get; private set; }
    public string? MercadoPagoPreferenceId { get; private set; }
    public string? BankTransferId { get; private set; }
    public string? UalaTransactionId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Payment() { }

    public static Payment Create(Guid orderId, decimal amount, PaymentMethod method)
    {
        var payment = new Payment();
        payment.Id = Guid.NewGuid();
        payment.OrderId = orderId;
        payment.Amount = amount;
        payment.Method = method;
        payment.Status = PaymentStatus.Pending;
        payment.CreatedAt = DateTime.UtcNow;
        payment.UpdatedAt = DateTime.UtcNow;
        return payment;
    }
}

using MiApp.Domain.Common;
using MiApp.Domain.Exceptions;

namespace MiApp.Domain.Entities;

public enum OrderStatus
{
    Draft = 1,
    Confirmed = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}

public class Order : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Draft;
    public DateTime CreatedAt { get; private set; }
    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();
    public decimal Total => Items.Sum(i => i.Total);

    private Order() { }

    public static Order Create(Guid customerId)
    {
        var order = new Order();
        order.Id = Guid.NewGuid();
        order.CustomerId = customerId;
        order.Status = OrderStatus.Draft;
        order.CreatedAt = DateTime.UtcNow;
        return order;
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        var item = OrderItem.Create(Id, productId, quantity, unitPrice);
        Items.Add(item);
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainRuleException("Only draft orders can be confirmed.");

        if (!Items.Any())
            throw new DomainRuleException("Cannot confirm an order with no items.");

        Status = OrderStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new DomainRuleException("Cannot cancel a delivered or already cancelled order.");

        Status = OrderStatus.Cancelled;
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new DomainRuleException("Only confirmed orders can be shipped.");

        Status = OrderStatus.Shipped;
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new DomainRuleException("Only shipped orders can be delivered.");

        Status = OrderStatus.Delivered;
    }
}

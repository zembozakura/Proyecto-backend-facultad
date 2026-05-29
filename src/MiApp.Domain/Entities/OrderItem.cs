using MiApp.Domain.Common;

namespace MiApp.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total => Quantity * UnitPrice;

    public Order? Order { get; private set; }
    public Product? Product { get; private set; }

    private OrderItem() { }

    public static OrderItem Create(Guid orderId, Guid productId, int quantity, decimal unitPrice)
    {
        var item = new OrderItem();
        item.Id = Guid.NewGuid();
        item.OrderId = orderId;
        item.ProductId = productId;
        item.Quantity = quantity;
        item.UnitPrice = unitPrice;
        return item;
    }
}

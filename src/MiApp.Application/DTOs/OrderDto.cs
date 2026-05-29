namespace MiApp.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = null!;
    public decimal Total { get; set; }
    public ICollection<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}

public class CreateOrderDto
{
    public Guid CustomerId { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class AddItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

namespace MiApp.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public ICollection<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
}

public class CreateOrderDto
{
    public string OrderNumber { get; set; } = null!;
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

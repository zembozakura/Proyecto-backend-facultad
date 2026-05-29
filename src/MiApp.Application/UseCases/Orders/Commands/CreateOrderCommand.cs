using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Orders.Commands;

public record CreateOrderCommand(
    Guid CustomerId,
    List<CreateOrderItemDto> Items) : IRequest<OrderDto>;

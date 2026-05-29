using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Orders.Commands;

public record AddItemToOrderCommand(Guid OrderId, Guid ProductId, int Quantity) : IRequest<OrderDto>;

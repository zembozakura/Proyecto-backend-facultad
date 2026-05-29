using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Orders.Commands;

public record CancelOrderCommand(Guid OrderId) : IRequest<OrderDto>;

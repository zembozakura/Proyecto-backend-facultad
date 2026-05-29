using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Orders.Commands;

public record ConfirmOrderCommand(Guid OrderId) : IRequest<OrderDto>;

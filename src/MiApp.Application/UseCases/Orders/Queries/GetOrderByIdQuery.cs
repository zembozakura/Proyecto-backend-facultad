using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Orders.Queries;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;

using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Orders.Queries;

public record GetAllOrdersQuery() : IRequest<IList<OrderDto>>;

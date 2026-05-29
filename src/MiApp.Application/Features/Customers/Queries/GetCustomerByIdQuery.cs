using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.Features.Customers.Queries;

public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerDto>;

using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Customers.Queries;

public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerDto>;

using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Customers.Commands;

public record RegisterCustomerCommand(string Name, string Email) : IRequest<CustomerDto>;

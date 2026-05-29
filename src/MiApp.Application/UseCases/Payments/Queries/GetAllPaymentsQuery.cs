using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Payments.Queries;

public record GetAllPaymentsQuery() : IRequest<IList<PaymentDto>>;

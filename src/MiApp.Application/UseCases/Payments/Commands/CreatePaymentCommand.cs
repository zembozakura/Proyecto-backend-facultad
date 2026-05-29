using MediatR;
using MiApp.Application.DTOs;
using MiApp.Domain.Entities;

namespace MiApp.Application.UseCases.Payments.Commands;

public record CreatePaymentCommand(
    Guid OrderId,
    decimal Amount,
    PaymentMethod Method) : IRequest<PaymentDto>;

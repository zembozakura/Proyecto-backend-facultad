using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Payments.Queries;

/// <summary>
/// Query para obtener un pago por su ID
/// Responsabilidad: Recuperar información sin cambiar el estado del sistema
/// </summary>
public record GetPaymentByIdQuery(Guid PaymentId) : IRequest<PaymentDto>;

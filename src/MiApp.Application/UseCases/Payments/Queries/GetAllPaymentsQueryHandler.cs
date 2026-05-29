using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MiApp.Application.DTOs;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.UseCases.Payments.Queries;

public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, IList<PaymentDto>>
{
    private readonly IPaymentRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllPaymentsQueryHandler> _logger;

    public GetAllPaymentsQueryHandler(
        IPaymentRepository repository,
        IMapper mapper,
        ILogger<GetAllPaymentsQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IList<PaymentDto>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Ejecutando caso de uso: Obtener todos los pagos");
        
        var payments = await _repository.GetAllAsync(cancellationToken);
        var paymentDtos = _mapper.Map<IList<PaymentDto>>(payments);
        
        _logger.LogInformation("Se obtuvieron {Count} pagos", paymentDtos.Count);
        
        return paymentDtos;
    }
}

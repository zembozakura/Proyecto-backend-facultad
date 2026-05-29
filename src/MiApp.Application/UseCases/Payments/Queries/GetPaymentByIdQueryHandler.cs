using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MiApp.Application.DTOs;
using MiApp.Domain.Entities;
using MiApp.Domain.Exceptions;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.UseCases.Payments.Queries;

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto>
{
    private readonly IPaymentRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetPaymentByIdQueryHandler> _logger;

    public GetPaymentByIdQueryHandler(
        IPaymentRepository repository,
        IMapper mapper,
        ILogger<GetPaymentByIdQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _repository.GetByIdAsync(request.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), request.PaymentId);

        return _mapper.Map<PaymentDto>(payment);
    }
}

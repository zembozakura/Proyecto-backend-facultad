using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MiApp.Application.DTOs;
using MiApp.Domain.Entities;
using MiApp.Domain.Exceptions;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.UseCases.Payments.Commands;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentDto>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreatePaymentCommandHandler> _logger;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreatePaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        _ = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException(nameof(Order), request.OrderId);

        var payment = Payment.Create(request.OrderId, request.Amount, request.Method);

        await _paymentRepository.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Payment {Id} created for order {OrderId}", payment.Id, request.OrderId);

        return _mapper.Map<PaymentDto>(payment);
    }
}

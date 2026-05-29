using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MiApp.Application.DTOs;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using MiApp.Domain.ValueObjects;

namespace MiApp.Application.Features.Customers.Commands;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, CustomerDto>
{
    private readonly ICustomerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisterCustomerCommandHandler> _logger;

    public RegisterCustomerCommandHandler(
        ICustomerRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<RegisterCustomerCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CustomerDto> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);
        var customer = Customer.Create(request.Name, email);

        await _repository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Customer {Id} registered", customer.Id);

        return _mapper.Map<CustomerDto>(customer);
    }
}

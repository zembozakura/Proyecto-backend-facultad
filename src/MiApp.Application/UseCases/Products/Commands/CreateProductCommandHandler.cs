using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MiApp.Application.DTOs;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.UseCases.Products.Commands;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateProductCommandHandler> logger)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating product {Name}", request.Name);

        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            request.CategoryId);

        await _repository.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product {Id} created", product.Id);

        return _mapper.Map<ProductDto>(product);
    }
}

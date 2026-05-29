using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MiApp.Application.DTOs;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.UseCases.Products.Queries;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IList<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;

    public GetAllProductsQueryHandler(
        IProductRepository repository,
        IMapper mapper,
        ILogger<GetAllProductsQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IList<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllWithCategoryAsync(cancellationToken);

        _logger.LogInformation("{Count} products retrieved", products.Count);

        return _mapper.Map<IList<ProductDto>>(products);
    }
}

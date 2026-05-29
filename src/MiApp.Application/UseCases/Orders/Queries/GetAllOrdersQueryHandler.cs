using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MiApp.Application.DTOs;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.UseCases.Orders.Queries;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IList<OrderDto>>
{
    private readonly IOrderRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetAllOrdersQueryHandler> _logger;

    public GetAllOrdersQueryHandler(
        IOrderRepository repository,
        IMapper mapper,
        ILogger<GetAllOrdersQueryHandler> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IList<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _repository.GetAllWithItemsAsync(cancellationToken);

        _logger.LogInformation("{Count} orders retrieved", orders.Count);

        return _mapper.Map<IList<OrderDto>>(orders);
    }
}

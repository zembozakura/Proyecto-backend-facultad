using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Products.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId) : IRequest<ProductDto>;

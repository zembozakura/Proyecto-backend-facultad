using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Products.Commands;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId) : IRequest<ProductDto>;

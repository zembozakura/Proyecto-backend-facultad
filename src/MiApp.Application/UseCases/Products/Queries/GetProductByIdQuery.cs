using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Products.Queries;

public record GetProductByIdQuery(Guid ProductId) : IRequest<ProductDto>;

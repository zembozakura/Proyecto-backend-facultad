using MediatR;

namespace MiApp.Application.UseCases.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest;

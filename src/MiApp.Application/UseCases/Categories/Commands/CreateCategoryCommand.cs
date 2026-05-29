using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.UseCases.Categories.Commands;

public record CreateCategoryCommand(string Name) : IRequest<CategoryDto>;

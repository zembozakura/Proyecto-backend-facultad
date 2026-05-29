using MediatR;
using MiApp.Application.DTOs;

namespace MiApp.Application.Features.Categories.Commands;

public record CreateCategoryCommand(string Name) : IRequest<CategoryDto>;

using MediatR;

namespace MiApp.Application.UseCases.Authentication.Commands;

public record LoginCommand(string Email, string Password) : IRequest<string?>;

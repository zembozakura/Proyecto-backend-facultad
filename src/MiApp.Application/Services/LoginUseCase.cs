using MiApp.Application.Contracts.Infrastructure;
using MiApp.Application.Interfaces;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.Services;

public class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUseCase(
        IUserRepository userRepository,
        ITokenService tokenService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<string?> ExecuteAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
            return null;

        if (!_passwordHasher.Verify(password, user.PasswordHash))
            return null;

        return _tokenService.GenerateToken(user);
    }
}

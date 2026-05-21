using System;
using System.Threading.Tasks;
using BCrypt.Net;
using MiApp.Application.Interfaces;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.Services
{
    public class LoginUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public LoginUseCase(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        }

        public async Task<(bool Success, string? Token, string Message)> ExecuteAsync(string email, string password)
        {
            // Validar entrada
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return (false, null, "Email y contraseña son requeridos");
            }

            // Buscar usuario por email
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                return (false, null, "Email o contraseña incorrectos");
            }

            // Verificar contraseña usando BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return (false, null, "Email o contraseña incorrectos");
            }

            // Generar token JWT
            var token = await _tokenService.GenerateTokenAsync(user);

            return (true, token, "Login exitoso");
        }
    }
}

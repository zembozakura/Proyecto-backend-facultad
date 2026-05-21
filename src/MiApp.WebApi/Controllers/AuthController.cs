using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApp.Application.Services;

namespace MiApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly LoginUseCase _loginUseCase;

        public AuthController(LoginUseCase loginUseCase)
        {
            _loginUseCase = loginUseCase ?? throw new ArgumentNullException(nameof(loginUseCase));
        }

        /// <summary>
        /// Login endpoint - Autentica usuario y retorna JWT
        /// 
        /// Request:
        /// POST /api/auth/login
        /// Content-Type: application/json
        /// 
        /// {
        ///   "email": "admin@example.com",
        ///   "password": "password123"
        /// }
        /// 
        /// Response 200 OK:
        /// {
        ///   "success": true,
        ///   "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///   "message": "Login exitoso"
        /// }
        /// 
        /// Response 401 Unauthorized:
        /// {
        ///   "success": false,
        ///   "token": null,
        ///   "message": "Email o contraseña incorrectos"
        /// }
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { success = false, message = "Request vacío" });
            }

            var (success, token, message) = await _loginUseCase.ExecuteAsync(request.Email, request.Password);

            if (success)
            {
                return Ok(new { success = true, token, message });
            }

            return Unauthorized(new { success = false, token = (string?)null, message });
        }

        /// <summary>
        /// Get current user info - Retorna información del usuario autenticado
        /// 
        /// Request:
        /// GET /api/auth/me
        /// Authorization: Bearer {token}
        /// 
        /// Response 200 OK:
        /// {
        ///   "id": "a1b2c3d4-...",
        ///   "name": "John Doe",
        ///   "email": "john@example.com",
        ///   "role": "Admin"
        /// }
        /// 
        /// Response 401 Unauthorized:
        /// "Unauthorized"
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var name = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            var email = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value;
            var role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            return Ok(new { id = userId, name, email, role });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}

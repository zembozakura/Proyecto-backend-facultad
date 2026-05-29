using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApp.Application.UseCases.Authentication.Commands;

namespace MiApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>POST /api/auth/login</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var token = await _mediator.Send(new LoginCommand(request.Email, request.Password), ct);

        if (token is null)
            return Unauthorized(new { message = "Credenciales incorrectas" });

        return Ok(new { token });
    }

    /// <summary>GET /api/auth/me — requiere token</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var name   = User.FindFirst(ClaimTypes.Name)?.Value;
        var email  = User.FindFirst(ClaimTypes.Email)?.Value;
        var role   = User.FindFirst(ClaimTypes.Role)?.Value;

        return Ok(new { id = userId, name, email, role });
    }
}

public record LoginRequest(string Email, string Password);

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApp.Application.DTOs;
using MiApp.Application.Features.Customers.Commands;
using MiApp.Application.Features.Customers.Queries;

namespace MiApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator) => _mediator = mediator;

    /// <summary>POST /api/customers — público (registro)</summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> Register([FromBody] RegisterCustomerDto dto, CancellationToken ct)
    {
        var customer = await _mediator.Send(new RegisterCustomerCommand(dto.Name, dto.Email), ct);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    /// <summary>GET /api/customers/{id} — requiere autenticación</summary>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id, CancellationToken ct) =>
        Ok(await _mediator.Send(new GetCustomerByIdQuery(id), ct));
}

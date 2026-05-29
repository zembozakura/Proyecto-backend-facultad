using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApp.Application.DTOs;
using MiApp.Application.UseCases.Payments.Commands;
using MiApp.Application.UseCases.Payments.Queries;

namespace MiApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator) => _mediator = mediator;

    /// <summary>GET /api/payments</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IList<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IList<PaymentDto>>> GetAll(CancellationToken ct) =>
        Ok(await _mediator.Send(new GetAllPaymentsQuery(), ct));

    /// <summary>GET /api/payments/{id}</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> GetById(Guid id, CancellationToken ct) =>
        Ok(await _mediator.Send(new GetPaymentByIdQuery(id), ct));

    /// <summary>POST /api/payments</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto dto, CancellationToken ct)
    {
        var payment = await _mediator.Send(
            new CreatePaymentCommand(dto.OrderId, dto.Amount, dto.Method), ct);

        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
    }
}

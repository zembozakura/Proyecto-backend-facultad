using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApp.Application.DTOs;
using MiApp.Application.UseCases.Orders.Commands;
using MiApp.Application.UseCases.Orders.Queries;

namespace MiApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator) => _mediator = mediator;

    /// <summary>GET /api/orders — Admin</summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IList<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IList<OrderDto>>> GetAll(CancellationToken ct) =>
        Ok(await _mediator.Send(new GetAllOrdersQuery(), ct));

    /// <summary>GET /api/orders/{id}</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken ct) =>
        Ok(await _mediator.Send(new GetOrderByIdQuery(id), ct));

    /// <summary>POST /api/orders — crea orden en estado Draft</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var order = await _mediator.Send(new CreateOrderCommand(dto.CustomerId, dto.Items), ct);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    /// <summary>POST /api/orders/{id}/confirm — Admin</summary>
    [HttpPost("{id:guid}/confirm")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<OrderDto>> Confirm(Guid id, CancellationToken ct) =>
        Ok(await _mediator.Send(new ConfirmOrderCommand(id), ct));

    /// <summary>POST /api/orders/{id}/cancel</summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<OrderDto>> Cancel(Guid id, CancellationToken ct) =>
        Ok(await _mediator.Send(new CancelOrderCommand(id), ct));

    /// <summary>POST /api/orders/{id}/items — agrega ítem a una orden Draft</summary>
    [HttpPost("{id:guid}/items")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<OrderDto>> AddItem(
        Guid id,
        [FromBody] AddItemDto dto,
        CancellationToken ct) =>
        Ok(await _mediator.Send(new AddItemToOrderCommand(id, dto.ProductId, dto.Quantity), ct));
}

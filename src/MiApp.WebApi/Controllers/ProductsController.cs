using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApp.Application.DTOs;
using MiApp.Application.UseCases.Products.Commands;
using MiApp.Application.UseCases.Products.Queries;

namespace MiApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator) => _mediator = mediator;

    /// <summary>GET /api/products — público</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IList<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IList<ProductDto>>> GetAll(CancellationToken ct) =>
        Ok(await _mediator.Send(new GetAllProductsQuery(), ct));

    /// <summary>GET /api/products/{id} — público</summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken ct) =>
        Ok(await _mediator.Send(new GetProductByIdQuery(id), ct));

    /// <summary>POST /api/products — Admin</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto, CancellationToken ct)
    {
        var product = await _mediator.Send(
            new CreateProductCommand(dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId), ct);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>PUT /api/products/{id} — Admin</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductDto>> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken ct) =>
        Ok(await _mediator.Send(
            new UpdateProductCommand(id, dto.Name, dto.Description, dto.Price, dto.Stock, dto.CategoryId), ct));

    /// <summary>DELETE /api/products/{id} — Admin (desactiva)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }
}

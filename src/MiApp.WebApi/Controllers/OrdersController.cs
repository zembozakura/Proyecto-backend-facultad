using MiApp.Application.DTOs;
using MiApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MiApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(OrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las órdenes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<OrderDto>>> GetAllOrders(CancellationToken ct)
    {
        _logger.LogInformation("Obteniendo todas las órdenes");
        var orders = await _orderService.GetAllOrdersAsync(ct);
        return Ok(orders);
    }

    /// <summary>
    /// Obtiene una orden por ID con sus items
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Obteniendo orden {OrderId}", id);
        var order = await _orderService.GetOrderAsync(id, ct);
        if (order == null)
            return NotFound(new { message = $"Orden {id} no encontrada" });

        return Ok(order);
    }

    /// <summary>
    /// Obtiene órdenes activas con paginación
    /// </summary>
    [HttpGet("active/paginated")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetActiveOrders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Obteniendo órdenes activas - Página: {PageNumber}, Tamaño: {PageSize}", pageNumber, pageSize);
        
        var (orders, total) = await _orderService.GetActiveOrdersAsync(pageNumber, pageSize, ct);
        return Ok(new
        {
            data = orders,
            pagination = new
            {
                pageNumber,
                pageSize,
                total,
                totalPages = (int)Math.Ceiling(total / (double)pageSize)
            }
        });
    }

    /// <summary>
    /// Crea una nueva orden (con items) en transacción atómica
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> CreateOrder(
        [FromBody] CreateOrderDto dto,
        CancellationToken ct)
    {
        _logger.LogInformation("Creando nueva orden: {OrderNumber}", dto.OrderNumber);
        
        try
        {
            var order = await _orderService.CreateOrderAsync(dto.OrderNumber, 
                dto.Items.Select(i => new MiApp.Domain.Entities.OrderItem 
                { 
                    ProductId = i.ProductId, 
                    Quantity = i.Quantity 
                }), ct);
            
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Cancela una orden
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CancelOrder(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Cancelando orden {OrderId}", id);
        
        try
        {
            await _orderService.CancelOrderAsync(id, ct);
            return Ok(new { message = "Orden cancelada exitosamente" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Orden {id} no encontrada" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

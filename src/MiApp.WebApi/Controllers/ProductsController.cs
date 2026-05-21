using FluentValidation;
using MiApp.Application.DTOs;
using MiApp.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MiApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]  // ✓ Requiere autenticación para todos los endpoints excepto los que especifiquen [AllowAnonymous]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductService productService,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los productos
    /// </summary>
    [HttpGet]
    [AllowAnonymous]  // ✓ Sin autenticación
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProductDto>>> GetAllProducts(CancellationToken ct)
    {
        _logger.LogInformation("Obteniendo todos los productos");
        var products = await _productService.GetAllProductsAsync(ct);
        return Ok(products);
    }

    /// <summary>
    /// Obtiene un producto por ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]  // ✓ Sin autenticación
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProduct(int id, CancellationToken ct)
    {
        _logger.LogInformation("Obteniendo producto {ProductId}", id);
        var product = await _productService.GetProductAsync(id, ct);
        if (product == null)
            return NotFound(new { message = $"Producto {id} no encontrado" });

        return Ok(product);
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]  // ✓ Solo Admin
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductDto dto,
        CancellationToken ct)
    {
        _logger.LogInformation("Creando nuevo producto: {ProductName}", dto.Name);
        
        var validationResult = await _createValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var product = await _productService.CreateProductAsync(dto, ct);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// Actualiza un producto existente
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        int id,
        [FromBody] UpdateProductDto dto,
        CancellationToken ct)
    {
        _logger.LogInformation("Actualizando producto {ProductId}", id);
        
        var validationResult = await _updateValidator.ValidateAsync(dto, ct);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        try
        {
            var product = await _productService.UpdateProductAsync(id, dto, ct);
            return Ok(product);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Producto {id} no encontrado" });
        }
    }

    /// <summary>
    /// Elimina un producto
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]  // ✓ Solo Admin
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteProduct(int id, CancellationToken ct)
    {
        _logger.LogInformation("Eliminando producto {ProductId}", id);
        
        try
        {
            await _productService.DeleteProductAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = $"Producto {id} no encontrado" });
        }
    }
}

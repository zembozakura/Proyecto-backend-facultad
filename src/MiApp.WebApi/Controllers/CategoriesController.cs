using AutoMapper;
using MiApp.Application.DTOs;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MiApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly IRepository<Category> _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        IRepository<Category> repository,
        IMapper mapper,
        ILogger<CategoriesController> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todas las categorías
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryDto>>> GetAllCategories(CancellationToken ct)
    {
        _logger.LogInformation("Obteniendo todas las categorías");
        var categories = await _repository.GetAllAsync(ct);
        var result = _mapper.Map<List<CategoryDto>>(categories);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene una categoría por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetCategory(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Obteniendo categoría {CategoryId}", id);
        var category = await _repository.GetByIdAsync(id, ct);
        if (category == null)
            return NotFound(new { message = $"Categoría {id} no encontrada" });

        return Ok(_mapper.Map<CategoryDto>(category));
    }

    /// <summary>
    /// Crea una nueva categoría
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> CreateCategory(
        [FromBody] CreateCategoryDto dto,
        CancellationToken ct)
    {
        _logger.LogInformation("Creando nueva categoría: {CategoryName}", dto.Name);
        
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "El nombre de la categoría es requerido" });

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = dto.Name
        };

        await _repository.AddAsync(category, ct);
        await _repository.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, _mapper.Map<CategoryDto>(category));
    }

    /// <summary>
    /// Actualiza una categoría
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(
        Guid id,
        [FromBody] CreateCategoryDto dto,
        CancellationToken ct)
    {
        _logger.LogInformation("Actualizando categoría {CategoryId}", id);
        
        var category = await _repository.GetByIdAsync(id, ct);
        if (category == null)
            return NotFound(new { message = $"Categoría {id} no encontrada" });

        category.Name = dto.Name;
        _repository.Update(category);
        await _repository.SaveChangesAsync(ct);

        return Ok(_mapper.Map<CategoryDto>(category));
    }

    /// <summary>
    /// Elimina una categoría
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteCategory(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Eliminando categoría {CategoryId}", id);
        
        var category = await _repository.GetByIdAsync(id, ct);
        if (category == null)
            return NotFound(new { message = $"Categoría {id} no encontrada" });

        _repository.Delete(category);
        await _repository.SaveChangesAsync(ct);

        return NoContent();
    }
}

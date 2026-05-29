using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiApp.Application.DTOs;
using MiApp.Application.UseCases.Categories.Commands;
using MiApp.Application.UseCases.Products.Queries;
using MiApp.Domain.Interfaces;
using AutoMapper;

namespace MiApp.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public CategoriesController(IMediator mediator, ICategoryRepository repository, IMapper mapper)
    {
        _mediator   = mediator;
        _repository = repository;
        _mapper     = mapper;
    }

    /// <summary>GET /api/categories — público</summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IList<CategoryDto>>> GetAll(CancellationToken ct)
    {
        var categories = await _repository.GetAllAsync(ct);
        return Ok(_mapper.Map<IList<CategoryDto>>(categories));
    }

    /// <summary>POST /api/categories — Admin</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto, CancellationToken ct)
    {
        var category = await _mediator.Send(new CreateCategoryCommand(dto.Name), ct);
        return CreatedAtAction(nameof(GetAll), category);
    }
}

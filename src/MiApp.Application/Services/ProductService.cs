using AutoMapper;
using MiApp.Application.DTOs;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;

namespace MiApp.Application.Services;

public interface IProductService
{
    Task<IList<ProductDto>> GetAllProductsAsync(CancellationToken ct = default);
    Task<ProductDto?> GetProductAsync(int productId, CancellationToken ct = default);
    Task<ProductDto> CreateProductAsync(CreateProductDto dto, CancellationToken ct = default);
    Task<ProductDto> UpdateProductAsync(int productId, UpdateProductDto dto, CancellationToken ct = default);
    Task DeleteProductAsync(int productId, CancellationToken ct = default);
}

public class ProductService : IProductService
{
    private readonly IRepository<Product> _repository;
    private readonly IMapper _mapper;

    public ProductService(IRepository<Product> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IList<ProductDto>> GetAllProductsAsync(CancellationToken ct = default)
    {
        var products = await _repository.GetAllAsync(ct);
        return _mapper.Map<IList<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductAsync(int productId, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(productId, ct);
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto, CancellationToken ct = default)
    {
        var product = _mapper.Map<Product>(dto);
        await _repository.AddAsync(product, ct);
        await _repository.SaveChangesAsync(ct);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateProductAsync(int productId, UpdateProductDto dto, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(productId, ct)
            ?? throw new KeyNotFoundException($"Producto {productId} no encontrado");

        _mapper.Map(dto, product);
        _repository.Update(product);
        await _repository.SaveChangesAsync(ct);

        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteProductAsync(int productId, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(productId, ct)
            ?? throw new KeyNotFoundException($"Producto {productId} no encontrado");

        _repository.Delete(product);
        await _repository.SaveChangesAsync(ct);
    }
}

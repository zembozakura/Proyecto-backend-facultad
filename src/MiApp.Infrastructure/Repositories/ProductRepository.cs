using Microsoft.EntityFrameworkCore;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using MiApp.Infrastructure.Data;

namespace MiApp.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Product?> GetByIdWithCategoryAsync(Guid id, CancellationToken ct = default) =>
        await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IList<Product>> GetAllWithCategoryAsync(CancellationToken ct = default) =>
        await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .ToListAsync(ct);

    public async Task<IList<Product>> GetActivesAsync(CancellationToken ct = default) =>
        await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Include(p => p.Category)
            .ToListAsync(ct);
}

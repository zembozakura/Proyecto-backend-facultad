using Microsoft.EntityFrameworkCore;
using MiApp.Domain.Entities;
using MiApp.Domain.Interfaces;
using MiApp.Infrastructure.Data;

namespace MiApp.Infrastructure.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Customer?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        await _context.Customers
            .FirstOrDefaultAsync(c => c.Email == email.ToLowerInvariant(), ct);
}

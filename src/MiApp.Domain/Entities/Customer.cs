using MiApp.Domain.Common;
using MiApp.Domain.ValueObjects;

namespace MiApp.Domain.Entities;

public class Customer : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Customer() { }

    public static Customer Create(string name, Email email)
    {
        var customer = new Customer();
        customer.Id = Guid.NewGuid();
        customer.Name = name;
        customer.Email = email.Value;
        customer.CreatedAt = DateTime.UtcNow;
        return customer;
    }
}

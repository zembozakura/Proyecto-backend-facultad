namespace MiApp.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    
    // Relación con productos (cuando los agreguemos)
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

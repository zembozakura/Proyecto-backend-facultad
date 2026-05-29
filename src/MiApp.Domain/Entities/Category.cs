using MiApp.Domain.Common;

namespace MiApp.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = null!;
    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { }

    public static Category Create(string name)
    {
        var category = new Category();
        category.Id = Guid.NewGuid();
        category.Name = name;
        return category;
    }
}

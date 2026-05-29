using MiApp.Domain.Common;
using MiApp.Domain.Exceptions;

namespace MiApp.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    private Product() { }

    public static Product Create(string name, string description, decimal price, int stock, Guid categoryId)
    {
        var product = new Product();
        product.Id = Guid.NewGuid();
        product.Name = name;
        product.Description = description;
        product.Price = price;
        product.Stock = stock;
        product.IsActive = true;
        product.CategoryId = categoryId;
        return product;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice <= 0)
            throw new DomainRuleException("Price must be greater than zero.");
        Price = newPrice;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainRuleException("Quantity to add must be greater than zero.");
        Stock += quantity;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainRuleException("Quantity to remove must be greater than zero.");
        if (quantity > Stock)
            throw new InsufficientStockException(quantity, Stock);
        Stock -= quantity;
    }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new DomainRuleException("Quantity to reserve must be greater than zero.");
        if (quantity > Stock)
            throw new InsufficientStockException(quantity, Stock);
        Stock -= quantity;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public void Update(string name, string description, decimal price, int stock, Guid categoryId)
    {
        Name = name;
        Description = description;
        UpdatePrice(price);
        Stock = stock;
        CategoryId = categoryId;
    }
}

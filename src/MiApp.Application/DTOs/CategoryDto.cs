namespace MiApp.Application.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

public class CreateCategoryDto
{
    public string Name { get; set; } = null!;
}

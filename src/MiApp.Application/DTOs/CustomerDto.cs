namespace MiApp.Application.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class RegisterCustomerDto
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
}

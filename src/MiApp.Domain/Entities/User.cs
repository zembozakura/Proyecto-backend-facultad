using MiApp.Domain.Common;

namespace MiApp.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Role { get; private set; } = "Customer";
    public DateTime CreatedAt { get; private set; }

    private User() { }

    public static User Create(string name, string email, string passwordHash, string role = "Customer")
    {
        var user = new User();
        user.Id = Guid.NewGuid();
        user.Name = name;
        user.Email = email;
        user.PasswordHash = passwordHash;
        user.Role = role;
        user.CreatedAt = DateTime.UtcNow;
        return user;
    }
}

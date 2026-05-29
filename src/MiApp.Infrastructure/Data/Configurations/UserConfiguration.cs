using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiApp.Domain.Entities;

namespace MiApp.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    private static readonly Guid AdminId = new("11111111-1111-1111-1111-111111111111");

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        // Admin seed — password: Admin1234!
        // Hash pre-generado con BCrypt.Net.BCrypt.HashPassword("Admin1234!")
        builder.HasData(new
        {
            Id           = AdminId,
            Name         = "Admin",
            Email        = "admin@miapp.com",
            PasswordHash = "$2a$11$Yw6S2lxWpVIGsAZtb/HFTO8O7lRDZw.OHT3F7pAMX.vF5mCo4YDPC",
            Role         = "Admin",
            CreatedAt    = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}

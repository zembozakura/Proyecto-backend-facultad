using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiApp.Domain.Entities;

namespace MiApp.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    // GUIDs FIJOS — nunca usar Guid.NewGuid() en HasData()
    private static readonly Guid ElectronicaId =
        new Guid("a1b2c3d4-0000-0000-0000-000000000001");
    private static readonly Guid RopaId =
        new Guid("a1b2c3d4-0000-0000-0000-000000000002");
    private static readonly Guid HogarId =
        new Guid("a1b2c3d4-0000-0000-0000-000000000003");

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

        // Datos iniciales — GUIDs hardcodeados obligatoriamente
        builder.HasData(
            new Category { Id = ElectronicaId, Name = "Electrónica" },
            new Category { Id = RopaId,        Name = "Ropa" },
            new Category { Id = HogarId,       Name = "Hogar" }
        );
    }
}

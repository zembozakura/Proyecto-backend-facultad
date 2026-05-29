using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiApp.Domain.Entities;

namespace MiApp.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    private static readonly Guid ElectronicaId = new("a1b2c3d4-0000-0000-0000-000000000001");
    private static readonly Guid RopaId        = new("a1b2c3d4-0000-0000-0000-000000000002");
    private static readonly Guid HogarId       = new("a1b2c3d4-0000-0000-0000-000000000003");

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasData(
            new { Id = ElectronicaId, Name = "Electrónica" },
            new { Id = RopaId,        Name = "Ropa" },
            new { Id = HogarId,       Name = "Hogar" });
    }
}

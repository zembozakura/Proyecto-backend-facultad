using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiApp.Domain.Entities;

namespace MiApp.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Currency)
            .HasMaxLength(3)
            .HasDefaultValue("ARS");

        builder.Property(p => p.Status)
            .HasConversion<int>();

        builder.Property(p => p.Method)
            .HasConversion<int>();

        builder.HasIndex(p => p.OrderId)
            .HasDatabaseName("IX_Payments_OrderId");
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiApp.Domain.Entities;

namespace MiApp.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de entidad Payment para Entity Framework Core
/// Define constrains, índices y propiedades específicas
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Tabla
        builder.ToTable("Payments");
        builder.HasKey(p => p.Id);

        // Propiedades
        builder.Property(p => p.OrderId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("ARS");

        builder.Property(p => p.Status)
            .IsRequired()
            .HasDefaultValue(PaymentStatus.Pending);

        builder.Property(p => p.Method)
            .IsRequired();

        builder.Property(p => p.MercadoPagoId)
            .HasMaxLength(100);

        builder.Property(p => p.MercadoPagoPreferenceId)
            .HasMaxLength(100);

        builder.Property(p => p.BankTransferId)
            .HasMaxLength(100);

        builder.Property(p => p.UalaTransactionId)
            .HasMaxLength(100);

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Relaciones
        builder.HasOne(p => p.Order)
            .WithMany(o => o.Payments)
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(p => p.OrderId)
            .HasDatabaseName("IX_Payment_OrderId");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("IX_Payment_Status");

        builder.HasIndex(p => p.MercadoPagoId)
            .HasDatabaseName("IX_Payment_MercadoPagoId");

        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("IX_Payment_CreatedAt");
    }
}

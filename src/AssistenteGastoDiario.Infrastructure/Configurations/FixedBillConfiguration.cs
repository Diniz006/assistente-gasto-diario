using AssistenteGastoDiario.Domain.Entities;
using AssistenteGastoDiario.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenteGastoDiario.Infrastructure.Configurations;

public sealed class FixedBillConfiguration : IEntityTypeConfiguration<FixedBill>
{
    public void Configure(EntityTypeBuilder<FixedBill> builder)
    {
        builder.ToTable("fixed_bills");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.CategoryId);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(120);
        builder.Property(x => x.Amount).IsRequired().HasPrecision(12, 2);
        builder.Property(x => x.DueDay).IsRequired();
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(FixedBillStatus.Pending);
        builder.Property(x => x.PaymentDate);
        builder.Property(x => x.IsRecurringMonthly).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.AutoIncludeInCycle).IsRequired().HasDefaultValue(true);
        builder.Property(x => x.Notes);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.Status });
        builder.HasIndex(x => new { x.UserId, x.DueDay });

        builder.HasOne(x => x.User)
            .WithMany(x => x.FixedBills)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.FixedBills)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

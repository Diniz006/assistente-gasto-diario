using AssistenteGastoDiario.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenteGastoDiario.Infrastructure.Configurations;

public sealed class MonthlyBudgetConfiguration : IEntityTypeConfiguration<MonthlyBudget>
{
    public void Configure(EntityTypeBuilder<MonthlyBudget> builder)
    {
        builder.ToTable("monthly_budgets");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.CategoryId).IsRequired();
        builder.Property(x => x.CycleStartDate).IsRequired();
        builder.Property(x => x.CycleEndDate).IsRequired();
        builder.Property(x => x.BudgetAmount).IsRequired().HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(x => x.SpentAmount).IsRequired().HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(x => x.Notes);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.CycleStartDate, x.CycleEndDate });
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => new { x.UserId, x.CategoryId, x.CycleStartDate, x.CycleEndDate }).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.MonthlyBudgets)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.MonthlyBudgets)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

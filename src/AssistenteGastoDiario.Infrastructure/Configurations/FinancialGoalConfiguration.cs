using AssistenteGastoDiario.Domain.Entities;
using AssistenteGastoDiario.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenteGastoDiario.Infrastructure.Configurations;

public sealed class FinancialGoalConfiguration : IEntityTypeConfiguration<FinancialGoal>
{
    public void Configure(EntityTypeBuilder<FinancialGoal> builder)
    {
        builder.ToTable("financial_goals");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.CategoryId);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(140);
        builder.Property(x => x.TargetAmount).IsRequired().HasPrecision(12, 2);
        builder.Property(x => x.CurrentAmount).IsRequired().HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(x => x.MonthlyPlannedAmount).IsRequired().HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(x => x.TargetDate);
        builder.Property(x => x.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(GoalPriority.Medium);
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(GoalStatus.Active);
        builder.Property(x => x.Notes);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.Status });
        builder.HasIndex(x => new { x.UserId, x.Priority });

        builder.HasOne(x => x.User)
            .WithMany(x => x.FinancialGoals)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.FinancialGoals)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

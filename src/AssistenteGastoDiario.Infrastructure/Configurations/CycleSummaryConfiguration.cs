using AssistenteGastoDiario.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenteGastoDiario.Infrastructure.Configurations;

public sealed class CycleSummaryConfiguration : IEntityTypeConfiguration<CycleSummary>
{
    public void Configure(EntityTypeBuilder<CycleSummary> builder)
    {
        builder.ToTable("cycle_summaries");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.CycleStartDate).IsRequired();
        builder.Property(x => x.CycleEndDate).IsRequired();
        builder.Property(x => x.IncomeTotal).IsRequired().HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(x => x.FixedBillsTotal).IsRequired().HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(x => x.ExpensesTotal).IsRequired().HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(x => x.GoalPlannedTotal).IsRequired().HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(x => x.GoalContributedTotal).IsRequired().HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(x => x.AvailableBalance).IsRequired().HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(x => x.SafeDailyLimit).IsRequired().HasPrecision(12, 2).HasDefaultValue(0m);
        builder.Property(x => x.DaysInCycle).IsRequired();
        builder.Property(x => x.DaysRemainingAtClose);
        builder.Property(x => x.ClosedAt);
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.CycleStartDate, x.CycleEndDate }).IsUnique();

        builder.HasOne(x => x.User)
            .WithMany(x => x.CycleSummaries)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

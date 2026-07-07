using AssistenteGastoDiario.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenteGastoDiario.Infrastructure.Configurations;

public sealed class GoalContributionConfiguration : IEntityTypeConfiguration<GoalContribution>
{
    public void Configure(EntityTypeBuilder<GoalContribution> builder)
    {
        builder.ToTable("goal_contributions");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.FinancialGoalId).IsRequired();
        builder.Property(x => x.Amount).IsRequired().HasPrecision(12, 2);
        builder.Property(x => x.ContributedOn).IsRequired();
        builder.Property(x => x.Notes);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => new { x.UserId, x.ContributedOn });
        builder.HasIndex(x => x.FinancialGoalId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.GoalContributions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.FinancialGoal)
            .WithMany(x => x.GoalContributions)
            .HasForeignKey(x => x.FinancialGoalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

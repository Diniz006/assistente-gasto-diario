using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Common;
using AssistenteGastoDiario.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssistenteGastoDiario.Infrastructure.Data;

public sealed class AppDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService? currentUserService = null)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<FinancialSetting> FinancialSettings => Set<FinancialSetting>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Income> Incomes => Set<Income>();
    public DbSet<FixedBill> FixedBills => Set<FixedBill>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<FinancialGoal> FinancialGoals => Set<FinancialGoal>();
    public DbSet<GoalContribution> GoalContributions => Set<GoalContribution>();
    public DbSet<MonthlyBudget> MonthlyBudgets => Set<MonthlyBudget>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<CycleSummary> CycleSummaries => Set<CycleSummary>();

    public override int SaveChanges()
    {
        ApplyTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        ApplyUserQueryFilters(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private void ApplyTimestamps()
    {
        var now = DateTimeOffset.UtcNow;

        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }

    private void ApplyUserQueryFilters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasQueryFilter(user => user.DeletedAt == null);

        ApplyUserQueryFilter<FinancialSetting>(modelBuilder);
        ApplyUserQueryFilter<Category>(modelBuilder);
        ApplyUserQueryFilter<Income>(modelBuilder);
        ApplyUserQueryFilter<FixedBill>(modelBuilder);
        ApplyUserQueryFilter<Expense>(modelBuilder);
        ApplyUserQueryFilter<FinancialGoal>(modelBuilder);
        ApplyUserQueryFilter<GoalContribution>(modelBuilder);
        ApplyUserQueryFilter<MonthlyBudget>(modelBuilder);
        ApplyUserQueryFilter<Alert>(modelBuilder);
        ApplyUserQueryFilter<CycleSummary>(modelBuilder);
    }

    private void ApplyUserQueryFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : class, IUserScopedEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(entity =>
            _currentUserService == null
                ? true
                : _currentUserService.UserId.HasValue && entity.UserId == _currentUserService.UserId.Value);
    }
}

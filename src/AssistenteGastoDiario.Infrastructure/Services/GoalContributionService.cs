using AssistenteGastoDiario.Application.DTOs.FinancialGoals;
using AssistenteGastoDiario.Application.DTOs.GoalContributions;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Entities;
using AssistenteGastoDiario.Domain.Enums;
using AssistenteGastoDiario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenteGastoDiario.Infrastructure.Services;

public sealed class GoalContributionService : IGoalContributionService
{
    private readonly AppDbContext _dbContext;
    private readonly IDashboardQueryService _dashboardQueryService;

    public GoalContributionService(AppDbContext dbContext, IDashboardQueryService dashboardQueryService)
    {
        _dbContext = dbContext;
        _dashboardQueryService = dashboardQueryService;
    }

    public async Task<IReadOnlyCollection<GoalContributionResponse>> ListByGoalAsync(
        Guid userId,
        Guid financialGoalId,
        CancellationToken cancellationToken = default)
    {
        var goalExists = await _dbContext.FinancialGoals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .AnyAsync(goal => goal.UserId == userId && goal.Id == financialGoalId, cancellationToken);

        if (!goalExists)
        {
            throw new InvalidOperationException("Financial goal not found.");
        }

        return await _dbContext.GoalContributions
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(contribution =>
                contribution.UserId == userId
                && contribution.FinancialGoalId == financialGoalId)
            .OrderByDescending(contribution => contribution.ContributedOn)
            .ThenByDescending(contribution => contribution.CreatedAt)
            .Select(contribution => Map(contribution))
            .ToListAsync(cancellationToken);
    }

    public async Task<GoalContributionCreatedResponse> CreateAsync(
        Guid userId,
        Guid financialGoalId,
        CreateGoalContributionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var goal = await _dbContext.FinancialGoals
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == financialGoalId, cancellationToken);

        if (goal is null)
        {
            throw new InvalidOperationException("Financial goal not found.");
        }

        if (goal.Status is GoalStatus.Canceled)
        {
            throw new InvalidOperationException("Canceled goals cannot receive contributions.");
        }

        var contributedOn = request.ContributedOn ?? DateOnly.FromDateTime(DateTime.Today);
        var contribution = new GoalContribution
        {
            UserId = userId,
            FinancialGoalId = financialGoalId,
            Amount = request.Amount,
            ContributedOn = contributedOn,
            Notes = request.Notes
        };

        goal.CurrentAmount += request.Amount;
        if (goal.CurrentAmount >= goal.TargetAmount)
        {
            goal.Status = GoalStatus.Completed;
        }

        _dbContext.GoalContributions.Add(contribution);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var dashboard = await _dashboardQueryService.GetCurrentAsync(userId, contributedOn, cancellationToken);

        return new GoalContributionCreatedResponse(
            Map(contribution),
            Map(goal),
            dashboard);
    }

    private static GoalContributionResponse Map(GoalContribution contribution) =>
        new(
            contribution.Id,
            contribution.UserId,
            contribution.FinancialGoalId,
            contribution.Amount,
            contribution.ContributedOn,
            contribution.Notes,
            contribution.CreatedAt,
            contribution.UpdatedAt);

    private static FinancialGoalResponse Map(FinancialGoal goal) =>
        new(
            goal.Id,
            goal.UserId,
            goal.CategoryId,
            goal.Name,
            goal.TargetAmount,
            goal.CurrentAmount,
            goal.MonthlyPlannedAmount,
            goal.TargetDate,
            goal.Priority,
            goal.Status,
            goal.Notes,
            goal.CreatedAt,
            goal.UpdatedAt);
}

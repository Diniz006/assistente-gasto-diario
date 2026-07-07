using AssistenteGastoDiario.Application.DTOs.FinancialGoals;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Entities;
using AssistenteGastoDiario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenteGastoDiario.Infrastructure.Services;

public sealed class FinancialGoalService : IFinancialGoalService
{
    private readonly AppDbContext _dbContext;

    public FinancialGoalService(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyCollection<FinancialGoalResponse>> ListAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _dbContext.FinancialGoals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(goal => goal.UserId == userId)
            .OrderBy(goal => goal.Priority)
            .ThenBy(goal => goal.TargetDate)
            .ThenBy(goal => goal.Name)
            .Select(goal => Map(goal))
            .ToListAsync(cancellationToken);

    public async Task<FinancialGoalResponse?> GetByIdAsync(Guid userId, Guid financialGoalId, CancellationToken cancellationToken = default)
    {
        var goal = await _dbContext.FinancialGoals
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == financialGoalId, cancellationToken);

        return goal is null ? null : Map(goal);
    }

    public async Task<FinancialGoalResponse> CreateAsync(Guid userId, CreateFinancialGoalRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await EnsureUserExistsAsync(userId, cancellationToken);

        var goal = new FinancialGoal
        {
            UserId = userId,
            CategoryId = request.CategoryId,
            Name = request.Name.Trim(),
            TargetAmount = request.TargetAmount,
            CurrentAmount = request.CurrentAmount,
            MonthlyPlannedAmount = request.MonthlyPlannedAmount,
            TargetDate = request.TargetDate,
            Priority = request.Priority,
            Status = request.Status,
            Notes = request.Notes
        };

        _dbContext.FinancialGoals.Add(goal);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(goal);
    }

    public async Task<FinancialGoalResponse?> UpdateAsync(Guid userId, Guid financialGoalId, UpdateFinancialGoalRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var goal = await _dbContext.FinancialGoals
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == financialGoalId, cancellationToken);

        if (goal is null)
        {
            return null;
        }

        goal.CategoryId = request.CategoryId;
        goal.Name = request.Name.Trim();
        goal.TargetAmount = request.TargetAmount;
        goal.CurrentAmount = request.CurrentAmount;
        goal.MonthlyPlannedAmount = request.MonthlyPlannedAmount;
        goal.TargetDate = request.TargetDate;
        goal.Priority = request.Priority;
        goal.Status = request.Status;
        goal.Notes = request.Notes;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(goal);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid financialGoalId, CancellationToken cancellationToken = default)
    {
        var goal = await _dbContext.FinancialGoals
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == financialGoalId, cancellationToken);

        if (goal is null)
        {
            return false;
        }

        _dbContext.FinancialGoals.Remove(goal);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (!await _dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken))
        {
            throw new InvalidOperationException("User not found.");
        }
    }

    private static FinancialGoalResponse Map(FinancialGoal goal) =>
        new(goal.Id, goal.UserId, goal.CategoryId, goal.Name, goal.TargetAmount, goal.CurrentAmount, goal.MonthlyPlannedAmount, goal.TargetDate, goal.Priority, goal.Status, goal.Notes, goal.CreatedAt, goal.UpdatedAt);
}

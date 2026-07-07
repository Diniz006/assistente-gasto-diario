using AssistenteGastoDiario.Application.DTOs.Incomes;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Entities;
using AssistenteGastoDiario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenteGastoDiario.Infrastructure.Services;

public sealed class IncomeService : IIncomeService
{
    private readonly AppDbContext _dbContext;

    public IncomeService(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyCollection<IncomeResponse>> ListByPeriodAsync(Guid userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default) =>
        await _dbContext.Incomes
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(income => income.UserId == userId && income.ReceivedOn >= startDate && income.ReceivedOn <= endDate)
            .OrderByDescending(income => income.ReceivedOn)
            .ThenBy(income => income.Description)
            .Select(income => Map(income))
            .ToListAsync(cancellationToken);

    public async Task<IncomeResponse?> GetByIdAsync(Guid userId, Guid incomeId, CancellationToken cancellationToken = default)
    {
        var income = await _dbContext.Incomes
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == incomeId, cancellationToken);

        return income is null ? null : Map(income);
    }

    public async Task<IncomeResponse> CreateAsync(Guid userId, CreateIncomeRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await EnsureUserExistsAsync(userId, cancellationToken);

        var income = new Income
        {
            UserId = userId,
            CategoryId = request.CategoryId,
            Description = request.Description.Trim(),
            Amount = request.Amount,
            ReceivedOn = request.ReceivedOn,
            IsRecurring = request.IsRecurring,
            Notes = request.Notes
        };

        _dbContext.Incomes.Add(income);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(income);
    }

    public async Task<IncomeResponse?> UpdateAsync(Guid userId, Guid incomeId, UpdateIncomeRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var income = await _dbContext.Incomes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == incomeId, cancellationToken);

        if (income is null)
        {
            return null;
        }

        income.CategoryId = request.CategoryId;
        income.Description = request.Description.Trim();
        income.Amount = request.Amount;
        income.ReceivedOn = request.ReceivedOn;
        income.IsRecurring = request.IsRecurring;
        income.Notes = request.Notes;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(income);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid incomeId, CancellationToken cancellationToken = default)
    {
        var income = await _dbContext.Incomes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == incomeId, cancellationToken);

        if (income is null)
        {
            return false;
        }

        _dbContext.Incomes.Remove(income);
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

    private static IncomeResponse Map(Income income) =>
        new(income.Id, income.UserId, income.CategoryId, income.Description, income.Amount, income.ReceivedOn, income.IsRecurring, income.Notes, income.CreatedAt, income.UpdatedAt);
}

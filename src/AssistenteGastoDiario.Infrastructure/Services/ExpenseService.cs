using AssistenteGastoDiario.Application.DTOs.Expenses;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Entities;
using AssistenteGastoDiario.Domain.Enums;
using AssistenteGastoDiario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenteGastoDiario.Infrastructure.Services;

public sealed class ExpenseService : IExpenseService
{
    private readonly AppDbContext _dbContext;

    public ExpenseService(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyCollection<ExpenseResponse>> ListByPeriodAsync(Guid userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default) =>
        await _dbContext.Expenses
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(expense => expense.UserId == userId && expense.SpentOn >= startDate && expense.SpentOn <= endDate)
            .OrderByDescending(expense => expense.SpentOn)
            .ThenBy(expense => expense.Description)
            .Select(expense => Map(expense))
            .ToListAsync(cancellationToken);

    public async Task<ExpenseResponse?> GetByIdAsync(Guid userId, Guid expenseId, CancellationToken cancellationToken = default)
    {
        var expense = await _dbContext.Expenses
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == expenseId, cancellationToken);

        return expense is null ? null : Map(expense);
    }

    public async Task<ExpenseResponse> CreateAsync(Guid userId, CreateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await EnsureUserExistsAsync(userId, cancellationToken);
        await EnsureExpenseCategoryIsValidAsync(userId, request.CategoryId, cancellationToken);

        var expense = new Expense
        {
            UserId = userId,
            CategoryId = request.CategoryId,
            Description = request.Description.Trim(),
            Amount = request.Amount,
            SpentOn = request.SpentOn,
            PaymentMethod = request.PaymentMethod,
            Notes = request.Notes
        };

        _dbContext.Expenses.Add(expense);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(expense);
    }

    public async Task<ExpenseResponse?> UpdateAsync(Guid userId, Guid expenseId, UpdateExpenseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var expense = await _dbContext.Expenses
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == expenseId, cancellationToken);

        if (expense is null)
        {
            return null;
        }

        await EnsureExpenseCategoryIsValidAsync(userId, request.CategoryId, cancellationToken);

        expense.CategoryId = request.CategoryId;
        expense.Description = request.Description.Trim();
        expense.Amount = request.Amount;
        expense.SpentOn = request.SpentOn;
        expense.PaymentMethod = request.PaymentMethod;
        expense.Notes = request.Notes;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(expense);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid expenseId, CancellationToken cancellationToken = default)
    {
        var expense = await _dbContext.Expenses
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == expenseId, cancellationToken);

        if (expense is null)
        {
            return false;
        }

        _dbContext.Expenses.Remove(expense);
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

    private async Task EnsureExpenseCategoryIsValidAsync(Guid userId, Guid categoryId, CancellationToken cancellationToken)
    {
        var isValid = await _dbContext.Categories
            .IgnoreQueryFilters()
            .AnyAsync(
                category =>
                    category.UserId == userId
                    && category.Id == categoryId
                    && category.Type == CategoryType.Expense
                    && category.IsActive,
                cancellationToken);

        if (!isValid)
        {
            throw new InvalidOperationException("Expense category not found or inactive.");
        }
    }

    private static ExpenseResponse Map(Expense expense) =>
        new(expense.Id, expense.UserId, expense.CategoryId, expense.Description, expense.Amount, expense.SpentOn, expense.PaymentMethod, expense.Notes, expense.CreatedAt, expense.UpdatedAt);
}

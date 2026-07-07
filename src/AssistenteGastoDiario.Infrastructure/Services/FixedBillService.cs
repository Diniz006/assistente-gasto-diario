using AssistenteGastoDiario.Application.DTOs.FixedBills;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Entities;
using AssistenteGastoDiario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenteGastoDiario.Infrastructure.Services;

public sealed class FixedBillService : IFixedBillService
{
    private readonly AppDbContext _dbContext;

    public FixedBillService(AppDbContext dbContext) => _dbContext = dbContext;

    public async Task<IReadOnlyCollection<FixedBillResponse>> ListAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await _dbContext.FixedBills
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(bill => bill.UserId == userId)
            .OrderBy(bill => bill.DueDay)
            .ThenBy(bill => bill.Name)
            .Select(bill => Map(bill))
            .ToListAsync(cancellationToken);

    public async Task<FixedBillResponse?> GetByIdAsync(Guid userId, Guid fixedBillId, CancellationToken cancellationToken = default)
    {
        var bill = await _dbContext.FixedBills
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == fixedBillId, cancellationToken);

        return bill is null ? null : Map(bill);
    }

    public async Task<FixedBillResponse> CreateAsync(Guid userId, CreateFixedBillRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await EnsureUserExistsAsync(userId, cancellationToken);

        var bill = new FixedBill
        {
            UserId = userId,
            CategoryId = request.CategoryId,
            Name = request.Name.Trim(),
            Amount = request.Amount,
            DueDay = request.DueDay,
            Status = request.Status,
            PaymentDate = request.PaymentDate,
            IsRecurringMonthly = request.IsRecurringMonthly,
            AutoIncludeInCycle = request.AutoIncludeInCycle,
            Notes = request.Notes
        };

        _dbContext.FixedBills.Add(bill);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(bill);
    }

    public async Task<FixedBillResponse?> UpdateAsync(Guid userId, Guid fixedBillId, UpdateFixedBillRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var bill = await _dbContext.FixedBills
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == fixedBillId, cancellationToken);

        if (bill is null)
        {
            return null;
        }

        bill.CategoryId = request.CategoryId;
        bill.Name = request.Name.Trim();
        bill.Amount = request.Amount;
        bill.DueDay = request.DueDay;
        bill.Status = request.Status;
        bill.PaymentDate = request.PaymentDate;
        bill.IsRecurringMonthly = request.IsRecurringMonthly;
        bill.AutoIncludeInCycle = request.AutoIncludeInCycle;
        bill.Notes = request.Notes;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Map(bill);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid fixedBillId, CancellationToken cancellationToken = default)
    {
        var bill = await _dbContext.FixedBills
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == fixedBillId, cancellationToken);

        if (bill is null)
        {
            return false;
        }

        _dbContext.FixedBills.Remove(bill);
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

    private static FixedBillResponse Map(FixedBill bill) =>
        new(bill.Id, bill.UserId, bill.CategoryId, bill.Name, bill.Amount, bill.DueDay, bill.Status, bill.PaymentDate, bill.IsRecurringMonthly, bill.AutoIncludeInCycle, bill.Notes, bill.CreatedAt, bill.UpdatedAt);
}

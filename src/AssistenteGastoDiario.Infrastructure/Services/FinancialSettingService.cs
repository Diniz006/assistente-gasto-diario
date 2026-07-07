using AssistenteGastoDiario.Application.DTOs.FinancialSettings;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Entities;
using AssistenteGastoDiario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenteGastoDiario.Infrastructure.Services;

public sealed class FinancialSettingService : IFinancialSettingService
{
    private readonly AppDbContext _dbContext;

    public FinancialSettingService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FinancialSettingResponse?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var setting = await _dbContext.FinancialSettings
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);

        return setting is null ? null : Map(setting);
    }

    public async Task<FinancialSettingResponse> UpsertAsync(
        Guid userId,
        UpsertFinancialSettingRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userExists = await _dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken);
        if (!userExists)
        {
            throw new InvalidOperationException("User not found.");
        }

        var setting = await _dbContext.FinancialSettings
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);

        if (setting is null)
        {
            setting = new FinancialSetting { UserId = userId };
            _dbContext.FinancialSettings.Add(setting);
        }

        setting.MonthlyIncomeDefault = request.MonthlyIncomeDefault;
        setting.CycleStartDay = request.CycleStartDay;
        setting.CurrencyCode = request.CurrencyCode.Trim().ToUpperInvariant();
        setting.MonthClosureDay = request.MonthClosureDay;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(setting);
    }

    private static FinancialSettingResponse Map(FinancialSetting setting) =>
        new(
            setting.Id,
            setting.UserId,
            setting.MonthlyIncomeDefault,
            setting.CycleStartDay,
            setting.CurrencyCode,
            setting.MonthClosureDay,
            setting.CreatedAt,
            setting.UpdatedAt);
}

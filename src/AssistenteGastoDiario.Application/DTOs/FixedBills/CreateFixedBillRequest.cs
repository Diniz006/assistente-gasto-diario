using System.ComponentModel.DataAnnotations;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.DTOs.FixedBills;

public sealed record CreateFixedBillRequest(
    Guid? CategoryId,
    [Required, MaxLength(120)] string Name,
    [Range(0.01, 9999999999.99)] decimal Amount,
    [Range(1, 31)] int DueDay,
    FixedBillStatus Status = FixedBillStatus.Pending,
    DateOnly? PaymentDate = null,
    bool IsRecurringMonthly = true,
    bool AutoIncludeInCycle = true,
    string? Notes = null);

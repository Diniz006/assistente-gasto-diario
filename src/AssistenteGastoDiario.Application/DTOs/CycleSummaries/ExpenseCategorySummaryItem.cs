namespace AssistenteGastoDiario.Application.DTOs.CycleSummaries;

public sealed record ExpenseCategorySummaryItem(
    Guid CategoryId,
    string CategoryName,
    decimal Amount,
    int ExpenseCount);

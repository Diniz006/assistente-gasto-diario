using AssistenteGastoDiario.Application.DTOs.Dashboard;
using AssistenteGastoDiario.Application.DTOs.Expenses;

namespace AssistenteGastoDiario.Application.DTOs.QuickExpenses;

public sealed record QuickExpenseResponse(
    ExpenseResponse Expense,
    DashboardSummaryResult? Dashboard);

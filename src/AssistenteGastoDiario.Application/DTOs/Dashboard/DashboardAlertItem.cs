using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.DTOs.Dashboard;

public sealed record DashboardAlertItem(
    AlertType Type,
    string Title,
    string Message);

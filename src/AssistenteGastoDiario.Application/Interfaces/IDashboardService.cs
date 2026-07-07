using AssistenteGastoDiario.Application.DTOs.Dashboard;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface IDashboardService
{
    DashboardSummaryResult Build(DashboardSummaryRequest request);
}

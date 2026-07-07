using AssistenteGastoDiario.Application.DTOs.GoalContributions;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface IGoalContributionService
{
    Task<IReadOnlyCollection<GoalContributionResponse>> ListByGoalAsync(
        Guid userId,
        Guid financialGoalId,
        CancellationToken cancellationToken = default);

    Task<GoalContributionCreatedResponse> CreateAsync(
        Guid userId,
        Guid financialGoalId,
        CreateGoalContributionRequest request,
        CancellationToken cancellationToken = default);
}

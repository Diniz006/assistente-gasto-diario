using System.ComponentModel.DataAnnotations;

namespace AssistenteGastoDiario.Application.DTOs.GoalContributions;

public sealed record CreateGoalContributionRequest(
    [Range(0.01, 9999999999.99)] decimal Amount,
    DateOnly? ContributedOn = null,
    string? Notes = null);

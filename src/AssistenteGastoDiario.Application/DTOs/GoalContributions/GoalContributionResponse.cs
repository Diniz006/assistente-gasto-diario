namespace AssistenteGastoDiario.Application.DTOs.GoalContributions;

public sealed record GoalContributionResponse(
    Guid Id,
    Guid UserId,
    Guid FinancialGoalId,
    decimal Amount,
    DateOnly ContributedOn,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

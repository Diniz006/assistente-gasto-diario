using AssistenteGastoDiario.Application.Interfaces;

namespace AssistenteGastoDiario.Infrastructure.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; set; }
}

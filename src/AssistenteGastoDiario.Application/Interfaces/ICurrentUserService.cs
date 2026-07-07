namespace AssistenteGastoDiario.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
}

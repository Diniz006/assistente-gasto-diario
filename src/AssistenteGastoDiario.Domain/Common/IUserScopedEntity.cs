namespace AssistenteGastoDiario.Domain.Common;

public interface IUserScopedEntity
{
    Guid UserId { get; set; }
}

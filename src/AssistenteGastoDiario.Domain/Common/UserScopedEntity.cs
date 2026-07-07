namespace AssistenteGastoDiario.Domain.Common;

public abstract class UserScopedEntity : EntityBase, IUserScopedEntity
{
    public Guid UserId { get; set; }
}

using AssistenteGastoDiario.Application.DTOs.Auth;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

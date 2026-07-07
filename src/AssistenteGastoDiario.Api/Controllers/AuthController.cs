using AssistenteGastoDiario.Application.DTOs.Auth;
using AssistenteGastoDiario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var auth = await _authService.LoginAsync(request, cancellationToken);

        return auth is null
            ? Unauthorized(new { message = "Invalid email or password." })
            : Ok(auth);
    }
}

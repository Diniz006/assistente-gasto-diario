using AssistenteGastoDiario.Application.DTOs.Auth;
using AssistenteGastoDiario.Application.DTOs.Users;
using AssistenteGastoDiario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Register), new { userId = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
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

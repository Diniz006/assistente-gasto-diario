using System.Security.Claims;
using AssistenteGastoDiario.Application.DTOs.Users;
using AssistenteGastoDiario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var user = await _userService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { userId = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{userId:guid}")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> GetById(
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (!RouteUserMatchesToken(userId))
        {
            return Forbid();
        }

        var user = await _userService.GetByIdAsync(userId, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("by-email")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> GetByEmail(
        [FromQuery] string email,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new { message = "Email is required." });
        }

        var user = await _userService.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        return RouteUserMatchesToken(user.Id) ? Ok(user) : Forbid();
    }

    private bool RouteUserMatchesToken(Guid userId)
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claimValue, out var authenticatedUserId) && authenticatedUserId == userId;
    }
}

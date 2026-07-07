using System.ComponentModel.DataAnnotations;

namespace AssistenteGastoDiario.Application.DTOs.Auth;

public sealed record LoginRequest(
    [Required, EmailAddress, MaxLength(255)] string Email,
    [Required, MinLength(8), MaxLength(100)] string Password);

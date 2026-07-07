using System.ComponentModel.DataAnnotations;

namespace AssistenteGastoDiario.Application.DTOs.Users;

public sealed record CreateUserRequest(
    [Required, MaxLength(120)] string Name,
    [Required, EmailAddress, MaxLength(255)] string Email,
    [Required, MinLength(8), MaxLength(100)] string Password);

using AssistenteGastoDiario.Application.DTOs.Users;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Entities;
using AssistenteGastoDiario.Domain.Enums;
using AssistenteGastoDiario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenteGastoDiario.Infrastructure.Services;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(AppDbContext dbContext, IPasswordHasher passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = NormalizeEmail(request.Email);
        var emailAlreadyExists = await _dbContext.Users
            .AnyAsync(user => user.Email == email, cancellationToken);

        if (emailAlreadyExists)
        {
            throw new InvalidOperationException("A user with this email already exists.");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            PasswordHash = _passwordHasher.Hash(request.Password)
        };

        _dbContext.Users.Add(user);
        _dbContext.Categories.AddRange(CreateDefaultCategories(user.Id));
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(user);
    }

    public async Task<UserResponse?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);

        return user is null ? null : Map(user);
    }

    public async Task<UserResponse?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Email == normalizedEmail, cancellationToken);

        return user is null ? null : Map(user);
    }

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static IReadOnlyCollection<Category> CreateDefaultCategories(Guid userId) =>
        new[]
        {
            CreateCategory(userId, "Salario", CategoryType.Income, "#2F855A", "wallet"),
            CreateCategory(userId, "Outras rendas", CategoryType.Income, "#38A169", "plus-circle"),
            CreateCategory(userId, "Mercado", CategoryType.Expense, "#DD6B20", "shopping-cart"),
            CreateCategory(userId, "Transporte", CategoryType.Expense, "#3182CE", "car"),
            CreateCategory(userId, "Lazer", CategoryType.Expense, "#805AD5", "smile"),
            CreateCategory(userId, "Moradia", CategoryType.Bill, "#C53030", "home"),
            CreateCategory(userId, "Servicos", CategoryType.Bill, "#D69E2E", "receipt"),
            CreateCategory(userId, "Reserva de emergencia", CategoryType.Goal, "#319795", "shield"),
            CreateCategory(userId, "Objetivo pessoal", CategoryType.Goal, "#718096", "target")
        };

    private static Category CreateCategory(
        Guid userId,
        string name,
        CategoryType type,
        string color,
        string icon) =>
        new()
        {
            UserId = userId,
            Name = name,
            Type = type,
            Color = color,
            Icon = icon,
            IsDefault = true,
            IsActive = true
        };

    private static UserResponse Map(User user) =>
        new(user.Id, user.Name, user.Email, user.CreatedAt, user.UpdatedAt);
}

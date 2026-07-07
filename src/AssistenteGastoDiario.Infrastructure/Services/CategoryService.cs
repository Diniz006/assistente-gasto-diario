using AssistenteGastoDiario.Application.DTOs.Categories;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Entities;
using AssistenteGastoDiario.Domain.Enums;
using AssistenteGastoDiario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenteGastoDiario.Infrastructure.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly AppDbContext _dbContext;

    public CategoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<CategoryResponse>> ListAsync(
        Guid userId,
        CategoryType? type = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Categories
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(category => category.UserId == userId);

        if (type.HasValue)
        {
            query = query.Where(category => category.Type == type.Value);
        }

        return await query
            .OrderBy(category => category.Type)
            .ThenBy(category => category.Name)
            .Select(category => Map(category))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryResponse?> GetByIdAsync(Guid userId, Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.Categories
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == categoryId, cancellationToken);

        return category is null ? null : Map(category);
    }

    public async Task<CategoryResponse> CreateAsync(
        Guid userId,
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await EnsureUserExistsAsync(userId, cancellationToken);
        await EnsureCategoryNameIsAvailableAsync(userId, request.Name, null, cancellationToken);

        var category = new Category
        {
            UserId = userId,
            Name = request.Name.Trim(),
            Type = request.Type,
            Color = request.Color?.Trim(),
            Icon = request.Icon?.Trim(),
            IsDefault = false,
            IsActive = true
        };

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task<CategoryResponse?> UpdateAsync(
        Guid userId,
        Guid categoryId,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var category = await _dbContext.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == categoryId, cancellationToken);

        if (category is null)
        {
            return null;
        }

        await EnsureCategoryNameIsAvailableAsync(userId, request.Name, categoryId, cancellationToken);

        category.Name = request.Name.Trim();
        category.Type = request.Type;
        category.Color = request.Color?.Trim();
        category.Icon = request.Icon?.Trim();
        category.IsActive = request.IsActive;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Map(category);
    }

    public async Task<bool> DeactivateAsync(Guid userId, Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _dbContext.Categories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(item => item.UserId == userId && item.Id == categoryId, cancellationToken);

        if (category is null)
        {
            return false;
        }

        category.IsActive = false;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private async Task EnsureUserExistsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.Users.AnyAsync(user => user.Id == userId, cancellationToken);
        if (!exists)
        {
            throw new InvalidOperationException("User not found.");
        }
    }

    private async Task EnsureCategoryNameIsAvailableAsync(
        Guid userId,
        string name,
        Guid? currentCategoryId,
        CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim();
        var exists = await _dbContext.Categories
            .IgnoreQueryFilters()
            .AnyAsync(
                category =>
                    category.UserId == userId
                    && category.Name == normalizedName
                    && (!currentCategoryId.HasValue || category.Id != currentCategoryId.Value),
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("A category with this name already exists.");
        }
    }

    private static CategoryResponse Map(Category category) =>
        new(
            category.Id,
            category.UserId,
            category.Name,
            category.Type,
            category.Color,
            category.Icon,
            category.IsDefault,
            category.IsActive,
            category.CreatedAt,
            category.UpdatedAt);
}

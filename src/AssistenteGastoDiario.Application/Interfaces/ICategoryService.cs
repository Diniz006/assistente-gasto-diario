using AssistenteGastoDiario.Application.DTOs.Categories;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyCollection<CategoryResponse>> ListAsync(Guid userId, CategoryType? type = null, CancellationToken cancellationToken = default);
    Task<CategoryResponse?> GetByIdAsync(Guid userId, Guid categoryId, CancellationToken cancellationToken = default);
    Task<CategoryResponse> CreateAsync(Guid userId, CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CategoryResponse?> UpdateAsync(Guid userId, Guid categoryId, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeactivateAsync(Guid userId, Guid categoryId, CancellationToken cancellationToken = default);
}

using AssistenteGastoDiario.Api.Filters;
using AssistenteGastoDiario.Application.DTOs.Categories;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Authorize]
[ServiceFilter(typeof(RequireMatchingUserIdFilter))]
[Route("api/users/{userId:guid}/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CategoryResponse>>> List(
        Guid userId,
        [FromQuery] CategoryType? type,
        CancellationToken cancellationToken)
    {
        var categories = await _categoryService.ListAsync(userId, type, cancellationToken);
        return Ok(categories);
    }

    [HttpGet("{categoryId:guid}")]
    public async Task<ActionResult<CategoryResponse>> GetById(
        Guid userId,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var category = await _categoryService.GetByIdAsync(userId, categoryId, cancellationToken);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create(
        Guid userId,
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryService.CreateAsync(userId, request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { userId, categoryId = category.Id }, category);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            return Conflict(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{categoryId:guid}")]
    public async Task<ActionResult<CategoryResponse>> Update(
        Guid userId,
        Guid categoryId,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryService.UpdateAsync(userId, categoryId, request, cancellationToken);
            return category is null ? NotFound() : Ok(category);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{categoryId:guid}")]
    public async Task<IActionResult> Deactivate(
        Guid userId,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var deactivated = await _categoryService.DeactivateAsync(userId, categoryId, cancellationToken);
        return deactivated ? NoContent() : NotFound();
    }
}

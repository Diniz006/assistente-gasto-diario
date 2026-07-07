using AssistenteGastoDiario.Api.Filters;
using AssistenteGastoDiario.Application.DTOs.FixedBills;
using AssistenteGastoDiario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Authorize]
[ServiceFilter(typeof(RequireMatchingUserIdFilter))]
[Route("api/users/{userId:guid}/fixed-bills")]
public sealed class FixedBillsController : ControllerBase
{
    private readonly IFixedBillService _fixedBillService;

    public FixedBillsController(IFixedBillService fixedBillService)
    {
        _fixedBillService = fixedBillService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<FixedBillResponse>>> List(Guid userId, CancellationToken cancellationToken)
    {
        var fixedBills = await _fixedBillService.ListAsync(userId, cancellationToken);
        return Ok(fixedBills);
    }

    [HttpGet("{fixedBillId:guid}")]
    public async Task<ActionResult<FixedBillResponse>> GetById(Guid userId, Guid fixedBillId, CancellationToken cancellationToken)
    {
        var fixedBill = await _fixedBillService.GetByIdAsync(userId, fixedBillId, cancellationToken);
        return fixedBill is null ? NotFound() : Ok(fixedBill);
    }

    [HttpPost]
    public async Task<ActionResult<FixedBillResponse>> Create(Guid userId, CreateFixedBillRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var fixedBill = await _fixedBillService.CreateAsync(userId, request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { userId, fixedBillId = fixedBill.Id }, fixedBill);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{fixedBillId:guid}")]
    public async Task<ActionResult<FixedBillResponse>> Update(Guid userId, Guid fixedBillId, UpdateFixedBillRequest request, CancellationToken cancellationToken)
    {
        var fixedBill = await _fixedBillService.UpdateAsync(userId, fixedBillId, request, cancellationToken);
        return fixedBill is null ? NotFound() : Ok(fixedBill);
    }

    [HttpDelete("{fixedBillId:guid}")]
    public async Task<IActionResult> Delete(Guid userId, Guid fixedBillId, CancellationToken cancellationToken)
    {
        var deleted = await _fixedBillService.DeleteAsync(userId, fixedBillId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}

using AssistenteGastoDiario.Application.DTOs.FixedBills;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface IFixedBillService
{
    Task<IReadOnlyCollection<FixedBillResponse>> ListAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<FixedBillResponse?> GetByIdAsync(Guid userId, Guid fixedBillId, CancellationToken cancellationToken = default);
    Task<FixedBillResponse> CreateAsync(Guid userId, CreateFixedBillRequest request, CancellationToken cancellationToken = default);
    Task<FixedBillResponse?> UpdateAsync(Guid userId, Guid fixedBillId, UpdateFixedBillRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid userId, Guid fixedBillId, CancellationToken cancellationToken = default);
}

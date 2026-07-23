using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Catalog.Dtos;

namespace PrintHub.Application.Features.Catalog;

public interface IServiceTypeAdminService
{
    Task<Result<IReadOnlyList<ServiceTypeAdminDto>>> ListAsync(CancellationToken ct = default);
    Task<Result<ServiceTypeAdminDto>> CreateAsync(CreateServiceTypeRequest request, CancellationToken ct = default);
    Task<Result<ServiceTypeAdminDto>> UpdateAsync(int id, UpdateServiceTypeRequest request, CancellationToken ct = default);
    Task<Result> DeactivateAsync(int id, CancellationToken ct = default);
}

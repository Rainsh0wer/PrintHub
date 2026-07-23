using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Documents.Dtos;

namespace PrintHub.Application.Features.Documents;

/// <summary>Customer document library (UC-12): upload, list, rename, delete.</summary>
public interface IDocumentService
{
    Task<Result<PagedResult<DocumentDto>>> ListAsync(int ownerId, PageRequest page, CancellationToken ct = default);
    Task<Result<DocumentDto>> UploadAsync(int ownerId, UploadDocumentInput input, CancellationToken ct = default);
    Task<Result<DocumentDto>> RenameAsync(int ownerId, int documentId, string fileName, CancellationToken ct = default);
    Task<Result> DeleteAsync(int ownerId, int documentId, CancellationToken ct = default);
}

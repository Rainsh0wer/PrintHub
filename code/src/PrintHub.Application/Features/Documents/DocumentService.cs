using System.Security.Cryptography;
using AutoMapper;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Documents.Dtos;
using PrintHub.Application.Specifications.Documents;
using PrintHub.Domain.Entities;

namespace PrintHub.Application.Features.Documents;

public class DocumentService : IDocumentService
{
    private const long MaxBytes = 25 * 1024 * 1024;
    private static readonly HashSet<string> AllowedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf", "image/png", "image/jpeg",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    };

    private readonly IUnitOfWork _uow;
    private readonly IFileStorage _storage;
    private readonly IMapper _mapper;

    public DocumentService(IUnitOfWork uow, IFileStorage storage, IMapper mapper)
    {
        _uow = uow;
        _storage = storage;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<DocumentDto>>> ListAsync(int ownerId, PageRequest page, CancellationToken ct = default)
    {
        var repo = _uow.Repository<DocumentFile>();
        var total = await repo.CountAsync(new DocumentsByOwnerCountSpecification(ownerId), ct);
        var docs = await repo.ListAsync(new DocumentsByOwnerSpecification(ownerId, page.Skip, page.Take), ct);

        var items = _mapper.Map<IReadOnlyList<DocumentDto>>(docs);
        return Result.Success(new PagedResult<DocumentDto>(items, total, page.PageNumber, page.PageSize));
    }

    public async Task<Result<DocumentDto>> UploadAsync(int ownerId, UploadDocumentInput input, CancellationToken ct = default)
    {
        if (input.Content.Length == 0)
            return Result<DocumentDto>.Fail("The file is empty.");
        if (input.Content.Length > MaxBytes)
            return Result<DocumentDto>.Fail("The file exceeds the 25 MB limit.");
        if (!AllowedTypes.Contains(input.ContentType))
            return Result<DocumentDto>.Fail("Only PDF, PNG, JPEG, or DOCX files are accepted.");
        if (!input.RightsDeclared)
            return Result<DocumentDto>.Fail("You must confirm you have the rights to print this file.");

        var path = await _storage.SaveAsync(input.Content, input.FileName, ownerId.ToString(), ct);
        var checksum = Convert.ToHexString(SHA256.HashData(input.Content));

        var doc = new DocumentFile
        {
            OwnerUserId = ownerId,
            FileName = input.FileName,
            StoragePath = path,
            ContentType = input.ContentType,
            FileSizeKb = input.Content.Length / 1024,
            DeclaredPageCount = input.DeclaredPageCount,
            RightsDeclared = input.RightsDeclared,
            Checksum = checksum,
            UploadedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        await _uow.Repository<DocumentFile>().AddAsync(doc, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Success(_mapper.Map<DocumentDto>(doc));
    }

    public async Task<Result<DocumentDto>> RenameAsync(int ownerId, int documentId, string fileName, CancellationToken ct = default)
    {
        var repo = _uow.Repository<DocumentFile>();
        var doc = await repo.GetByIdAsync(documentId, ct);
        if (doc is null || doc.OwnerUserId != ownerId || doc.IsDeleted)
            return Result<DocumentDto>.NotFound("Document not found.");

        doc.FileName = fileName;
        repo.Update(doc);
        await _uow.SaveChangesAsync(ct);
        return Result.Success(_mapper.Map<DocumentDto>(doc));
    }

    public async Task<Result> DeleteAsync(int ownerId, int documentId, CancellationToken ct = default)
    {
        var repo = _uow.Repository<DocumentFile>();
        var doc = await repo.GetByIdAsync(documentId, ct);
        if (doc is null || doc.OwnerUserId != ownerId || doc.IsDeleted)
            return Result.NotFound("Document not found.");

        if (await _uow.Repository<OrderItem>().AnyAsync(new ActiveOrderItemsByDocumentSpecification(documentId), ct))
            return Result.Conflict("This document is used by an active order and cannot be deleted.");

        doc.IsDeleted = true;
        repo.Update(doc);
        await _uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}

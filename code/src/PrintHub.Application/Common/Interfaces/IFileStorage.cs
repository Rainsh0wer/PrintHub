namespace PrintHub.Application.Common.Interfaces;

/// <summary>Stores uploaded document bytes and returns a relative path. Implemented
/// in Infrastructure (local disk here; swappable for blob storage).</summary>
public interface IFileStorage
{
    Task<string> SaveAsync(byte[] content, string fileName, string subfolder, CancellationToken ct = default);
    Task DeleteAsync(string relativePath, CancellationToken ct = default);
}

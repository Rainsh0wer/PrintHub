using Microsoft.Extensions.Configuration;
using PrintHub.Application.Common.Interfaces;

namespace PrintHub.Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private readonly string _root;

    public LocalFileStorage(IConfiguration configuration)
    {
        _root = configuration["Storage:UploadsPath"] ?? "uploads";
        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveAsync(byte[] content, string fileName, string subfolder, CancellationToken ct = default)
    {
        var safeName = string.Concat(fileName.Split(Path.GetInvalidFileNameChars()));
        var folder = Path.Combine(_root, subfolder);
        Directory.CreateDirectory(folder);

        var relative = Path.Combine(subfolder, $"{Guid.NewGuid():N}_{safeName}");
        await File.WriteAllBytesAsync(Path.Combine(_root, relative), content, ct);
        return relative.Replace('\\', '/');
    }

    public Task DeleteAsync(string relativePath, CancellationToken ct = default)
    {
        var full = Path.Combine(_root, relativePath);
        if (File.Exists(full)) File.Delete(full);
        return Task.CompletedTask;
    }
}

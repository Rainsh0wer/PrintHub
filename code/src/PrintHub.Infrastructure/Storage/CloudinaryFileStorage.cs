using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using PrintHub.Application.Common.Interfaces;

namespace PrintHub.Infrastructure.Storage;

/// <summary>Stores uploads on Cloudinary and returns the secure URL. Selected over
/// <see cref="LocalFileStorage"/> when the Cloudinary section is configured.</summary>
public class CloudinaryFileStorage : IFileStorage
{
    private static readonly string[] ImageExtensions = { ".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp" };

    private readonly Cloudinary _cloudinary;

    public CloudinaryFileStorage(IOptions<CloudinaryOptions> options)
    {
        var o = options.Value;
        _cloudinary = new Cloudinary(new Account(o.CloudName, o.ApiKey, o.ApiSecret));
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> SaveAsync(byte[] content, string fileName, string subfolder, CancellationToken ct = default)
    {
        using var stream = new MemoryStream(content);
        var folder = $"printhub/{subfolder}";
        var file = new FileDescription(fileName, stream);

        if (ImageExtensions.Contains(Path.GetExtension(fileName).ToLowerInvariant()))
        {
            var result = await _cloudinary.UploadAsync(new ImageUploadParams
            {
                File = file, Folder = folder, UniqueFilename = true, Overwrite = false
            }, ct);
            return result.SecureUrl?.ToString()
                   ?? throw new InvalidOperationException(result.Error?.Message ?? "Cloudinary upload failed.");
        }

        var raw = await _cloudinary.UploadAsync(new RawUploadParams
        {
            File = file, Folder = folder, UniqueFilename = true, Overwrite = false
        });
        return raw.SecureUrl?.ToString()
               ?? throw new InvalidOperationException(raw.Error?.Message ?? "Cloudinary upload failed.");
    }

    public async Task DeleteAsync(string relativePath, CancellationToken ct = default)
    {
        var publicId = ExtractPublicId(relativePath);
        if (publicId is null) return;
        try { await _cloudinary.DestroyAsync(new DeletionParams(publicId)); } catch { /* best-effort */ }
    }

    private static string? ExtractPublicId(string url)
    {
        const string marker = "/upload/";
        var idx = url.IndexOf(marker, StringComparison.Ordinal);
        if (idx < 0) return null;

        var segments = url[(idx + marker.Length)..].Split('/')
            .SkipWhile(s => s.Length > 1 && s[0] == 'v' && s[1..].All(char.IsDigit));
        var joined = string.Join('/', segments);
        var dot = joined.LastIndexOf('.');
        return dot > 0 ? joined[..dot] : joined;
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Api.Common;
using PrintHub.Application.Common.Interfaces;
using PrintHub.Application.Common.Models;
using PrintHub.Application.Features.Documents;
using PrintHub.Application.Features.Documents.Dtos;

namespace PrintHub.Api.Controllers;

/// <summary>Customer document library (UC-12).</summary>
[ApiController]
[Route("api/documents")]
[Authorize]
[Produces("application/json")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documents;
    private readonly ICurrentUser _currentUser;

    public DocumentsController(IDocumentService documents, ICurrentUser currentUser)
    {
        _documents = documents;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] PageRequest page, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _documents.ListAsync(userId.Value, page, ct)).ToActionResult();
    }

    [HttpPost]
    [RequestSizeLimit(26 * 1024 * 1024)]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] int? declaredPageCount,
        [FromForm] bool rightsDeclared, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        if (file is null || file.Length == 0) return BadRequest(ApiResponse.Fail("No file was uploaded."));

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);
        var input = new UploadDocumentInput(file.FileName, file.ContentType, ms.ToArray(), declaredPageCount, rightsDeclared);

        return (await _documents.UploadAsync(userId.Value, input, ct))
            .ToActionResult(StatusCodes.Status201Created, "File uploaded.");
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Rename(int id, RenameDocumentRequest request, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _documents.RenameAsync(userId.Value, id, request.FileName, ct)).ToActionResult(successMessage: "File renamed.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var userId = _currentUser.UserId;
        if (userId is null) return Unauthorized(ApiResponse.Fail("Not authenticated."));
        return (await _documents.DeleteAsync(userId.Value, id, ct)).ToActionResult(successMessage: "File deleted.");
    }
}

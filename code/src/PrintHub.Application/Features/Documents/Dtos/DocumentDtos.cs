namespace PrintHub.Application.Features.Documents.Dtos;

public record DocumentDto(
    int Id,
    string FileName,
    string ContentType,
    long FileSizeKb,
    int? DeclaredPageCount,
    bool RightsDeclared,
    DateTime UploadedAt);

/// <summary>Buffered upload passed from the controller to the service.</summary>
public record UploadDocumentInput(
    string FileName,
    string ContentType,
    byte[] Content,
    int? DeclaredPageCount,
    bool RightsDeclared);

public record RenameDocumentRequest(string FileName);

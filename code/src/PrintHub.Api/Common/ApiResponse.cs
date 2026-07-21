namespace PrintHub.Api.Common;

/// <summary>
/// Uniform envelope for every API response (response-wrapper pattern). Success
/// carries Data; failure carries a Message and optional field-level Errors.
/// </summary>
public class ApiResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public object? Data { get; init; }
    public IEnumerable<string>? Errors { get; init; }

    public static ApiResponse Ok(object? data = null, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse Fail(string message, IEnumerable<string>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}

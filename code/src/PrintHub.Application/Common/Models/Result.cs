namespace PrintHub.Application.Common.Models;

/// <summary>
/// Classifies a failure so the API layer can map it to the right HTTP status
/// code without the service layer knowing about HTTP.
/// </summary>
public enum ErrorType
{
    None = 0,
    Validation = 1,   // 400
    Unauthorized = 2, // 401
    Forbidden = 3,    // 403
    NotFound = 4,     // 404
    Conflict = 5,     // 409
    Unexpected = 6    // 500
}

/// <summary>
/// Result pattern: represents the outcome of a use case without throwing for
/// expected business failures. Keeps controllers thin — they inspect the result
/// and translate <see cref="ErrorType"/> to a status code.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public ErrorType ErrorType { get; }

    protected Result(bool isSuccess, string? error, ErrorType errorType)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorType = errorType;
    }

    public static Result Success() => new(true, null, ErrorType.None);
    public static Result Failure(string error, ErrorType type = ErrorType.Validation) => new(false, error, type);

    public static Result NotFound(string error = "Resource not found.") => Failure(error, ErrorType.NotFound);
    public static Result Forbidden(string error = "You do not have permission to access this resource.") => Failure(error, ErrorType.Forbidden);
    public static Result Conflict(string error) => Failure(error, ErrorType.Conflict);

    public static Result<T> Success<T>(T value) => Result<T>.Ok(value);
    public static Result<T> Failure<T>(string error, ErrorType type = ErrorType.Validation) => Result<T>.Fail(error, type);
}

/// <summary>Result carrying a value on success.</summary>
public sealed class Result<T> : Result
{
    public T? Value { get; }

    private Result(bool isSuccess, T? value, string? error, ErrorType errorType)
        : base(isSuccess, error, errorType)
    {
        Value = value;
    }

    public static Result<T> Ok(T value) => new(true, value, null, ErrorType.None);
    public static Result<T> Fail(string error, ErrorType type = ErrorType.Validation) => new(false, default, error, type);

    public static new Result<T> NotFound(string error = "Resource not found.") => Fail(error, ErrorType.NotFound);
    public static new Result<T> Forbidden(string error = "You do not have permission to access this resource.") => Fail(error, ErrorType.Forbidden);
    public static new Result<T> Conflict(string error) => Fail(error, ErrorType.Conflict);
}

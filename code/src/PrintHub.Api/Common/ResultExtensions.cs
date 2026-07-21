using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrintHub.Application.Common.Models;

namespace PrintHub.Api.Common;

/// <summary>
/// Translates a service-layer <see cref="Result"/> into an HTTP response wrapped
/// in <see cref="ApiResponse"/>. This is the single place ErrorType is mapped to
/// a status code, keeping controllers free of that concern.
/// </summary>
public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result, int successStatus = StatusCodes.Status200OK, string? successMessage = null)
        => result.IsSuccess
            ? new ObjectResult(ApiResponse.Ok(result.Value, successMessage)) { StatusCode = successStatus }
            : Problem(result);

    public static IActionResult ToActionResult(
        this Result result, int successStatus = StatusCodes.Status200OK, string? successMessage = null)
        => result.IsSuccess
            ? new ObjectResult(ApiResponse.Ok(null, successMessage)) { StatusCode = successStatus }
            : Problem(result);

    private static IActionResult Problem(Result result)
    {
        var status = result.ErrorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
        return new ObjectResult(ApiResponse.Fail(result.Error ?? "Request failed.")) { StatusCode = status };
    }
}

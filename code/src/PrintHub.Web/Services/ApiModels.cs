namespace PrintHub.Web.Services;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public string[]? Errors { get; set; }
}

public record ApiResult<T>(bool Ok, T? Data, string? Error, int Status)
{
    public static ApiResult<T> Fail(string error, int status = 0) => new(false, default, error, status);
}

public static class SessionKeys
{
    public const string AccessToken = "access_token";
    public const string RefreshToken = "refresh_token";
    public const string UserName = "user_name";
    public const string UserEmail = "user_email";
    public const string UserRole = "user_role";
    public const string ShopIds = "shop_ids";
}

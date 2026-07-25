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

public class MeDto
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public int[]? ShopIds { get; set; }
}

public static class SessionKeys
{
    public const string AccessToken = "access_token";
    public const string RefreshToken = "refresh_token";
    public const string UserName = "user_name";
    public const string UserEmail = "user_email";
    public const string UserRole = "user_role";
    public const string UserAvatar = "user_avatar";
    public const string ShopIds = "shop_ids";
}

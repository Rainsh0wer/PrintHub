using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace PrintHub.Web.Services;

public class PrintHubApiClient
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly IHttpClientFactory _factory;
    private readonly IHttpContextAccessor _http;

    public PrintHubApiClient(IHttpClientFactory factory, IHttpContextAccessor http)
    {
        _factory = factory;
        _http = http;
    }

    public Task<ApiResult<T>> GetAsync<T>(string path, CancellationToken ct = default)
        => SendAsync<T>(HttpMethod.Get, path, null, ct);

    public Task<ApiResult<T>> PostAsync<T>(string path, object? body, CancellationToken ct = default)
        => SendAsync<T>(HttpMethod.Post, path, body, ct);

    public Task<ApiResult<T>> PutAsync<T>(string path, object? body, CancellationToken ct = default)
        => SendAsync<T>(HttpMethod.Put, path, body, ct);

    public Task<ApiResult<object>> DeleteAsync(string path, CancellationToken ct = default)
        => SendAsync<object>(HttpMethod.Delete, path, null, ct);

    public async Task<ApiResult<T>> PostFileAsync<T>(string path, IFormFile file, IEnumerable<KeyValuePair<string, string>> fields, CancellationToken ct = default)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            var sc = new StreamContent(file.OpenReadStream());
            sc.Headers.ContentType = new MediaTypeHeaderValue(string.IsNullOrEmpty(file.ContentType) ? "application/octet-stream" : file.ContentType);
            content.Add(sc, "file", file.FileName);
            foreach (var f in fields) content.Add(new StringContent(f.Value), f.Key);

            using var request = new HttpRequestMessage(HttpMethod.Post, path) { Content = content };
            var token = _http.HttpContext?.Session.GetString(SessionKeys.AccessToken);
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var client = _factory.CreateClient("api");
            using var response = await client.SendAsync(request, ct);
            var payload = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(Json, ct);
            if (response.IsSuccessStatusCode)
                return new ApiResult<T>(true, payload is null ? default : payload.Data, null, (int)response.StatusCode);
            return new ApiResult<T>(false, default, payload?.Message ?? $"Upload failed ({(int)response.StatusCode}).", (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Fail($"Upload failed: {ex.Message}");
        }
    }

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string path, object? body, CancellationToken ct)
    {
        try
        {
            var result = await SendOnceAsync<T>(method, path, body, ct);

            // The access token lives ~15 minutes. Rather than dropping the user at a
            // login screen mid-task, spend the stored refresh token once and replay
            // the original request.
            if (result.Status == StatusCodes.Status401Unauthorized && await TryRefreshAsync(ct))
                result = await SendOnceAsync<T>(method, path, body, ct);

            return result;
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Fail($"Could not reach the API — make sure it is running on :5080. ({ex.Message})");
        }
    }

    private async Task<ApiResult<T>> SendOnceAsync<T>(HttpMethod method, string path, object? body, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(method, path);
        if (body is not null)
            request.Content = JsonContent.Create(body, options: Json);

        var token = _http.HttpContext?.Session.GetString(SessionKeys.AccessToken);
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var client = _factory.CreateClient("api");
        using var response = await client.SendAsync(request, ct);
        var status = (int)response.StatusCode;

        // Not every response carries the ApiResponse envelope: the JWT middleware
        // answers 401/403 with an empty body. Parsing must not turn that into a
        // "cannot reach the API" error, and the status has to survive so the caller
        // can react to it (e.g. refresh the token and retry).
        var payload = await TryReadPayloadAsync<T>(response, ct);

        if (response.IsSuccessStatusCode)
            return new ApiResult<T>(true, payload is null ? default : payload.Data, null, status);

        return new ApiResult<T>(false, default, BuildError(payload, status), status);
    }

    private static async Task<ApiResponse<T>?> TryReadPayloadAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            if (response.Content.Headers.ContentLength == 0) return null;
            return await response.Content.ReadFromJsonAsync<ApiResponse<T>>(Json, ct);
        }
        catch (JsonException)
        {
            return null;    // a non-JSON error body is not worth failing the request over
        }
        catch (NotSupportedException)
        {
            return null;    // unexpected content type
        }
    }

    /// <summary>
    /// Exchanges the stored refresh token for a fresh pair and writes both back to the
    /// session. Returns false when there is nothing to refresh with or the token has
    /// been revoked, in which case the caller surfaces the original 401.
    /// </summary>
    private async Task<bool> TryRefreshAsync(CancellationToken ct)
    {
        var session = _http.HttpContext?.Session;
        var refreshToken = session?.GetString(SessionKeys.RefreshToken);
        if (session is null || string.IsNullOrEmpty(refreshToken)) return false;

        try
        {
            var client = _factory.CreateClient("api");
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/auth/refresh")
            {
                Content = JsonContent.Create(new { refreshToken }, options: Json)
            };

            using var response = await client.SendAsync(request, ct);
            if (!response.IsSuccessStatusCode) return false;

            var payload = await response.Content.ReadFromJsonAsync<ApiResponse<AuthTokens>>(Json, ct);
            if (payload?.Data is not { } tokens || string.IsNullOrEmpty(tokens.AccessToken)) return false;

            session.SetString(SessionKeys.AccessToken, tokens.AccessToken);
            if (!string.IsNullOrEmpty(tokens.RefreshToken))
                session.SetString(SessionKeys.RefreshToken, tokens.RefreshToken);
            return true;
        }
        catch
        {
            return false;   // treat any refresh failure as "not signed in"
        }
    }

    /// <summary>Only the token pair is needed here, so the full AuthResponse is not deserialised.</summary>
    private sealed record AuthTokens(string AccessToken, string RefreshToken);

    /// <summary>
    /// Builds the message shown to the user. Validation failures carry the useful
    /// detail in <c>Errors</c> while <c>Message</c> is only a generic header, so the
    /// field errors are appended rather than discarded — otherwise every failed form
    /// just says "Validation failed." with no indication of what to fix.
    /// </summary>
    private static string BuildError<T>(ApiResponse<T>? payload, int statusCode)
    {
        var details = payload?.Errors is { Length: > 0 } errors
            ? string.Join(" ", errors.Where(e => !string.IsNullOrWhiteSpace(e)))
            : null;

        if (!string.IsNullOrWhiteSpace(payload?.Message) && !string.IsNullOrWhiteSpace(details))
            return $"{payload!.Message} {details}";

        if (!string.IsNullOrWhiteSpace(payload?.Message)) return payload!.Message!;
        if (!string.IsNullOrWhiteSpace(details)) return details!;

        // Empty-bodied responses come from the auth middleware, not the services.
        return statusCode switch
        {
            StatusCodes.Status401Unauthorized => "Your session has expired. Please sign in again.",
            StatusCodes.Status403Forbidden => "You do not have permission to do that.",
            _ => $"Request failed ({statusCode})."
        };
    }
}

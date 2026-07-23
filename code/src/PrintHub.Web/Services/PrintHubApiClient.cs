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

    private async Task<ApiResult<T>> SendAsync<T>(HttpMethod method, string path, object? body, CancellationToken ct)
    {
        try
        {
            using var request = new HttpRequestMessage(method, path);
            if (body is not null)
                request.Content = JsonContent.Create(body, options: Json);

            var token = _http.HttpContext?.Session.GetString(SessionKeys.AccessToken);
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var client = _factory.CreateClient("api");
            using var response = await client.SendAsync(request, ct);

            var payload = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(Json, ct);
            if (response.IsSuccessStatusCode)
                return new ApiResult<T>(true, payload is null ? default : payload.Data, null, (int)response.StatusCode);

            var message = payload?.Message
                ?? (payload?.Errors is { Length: > 0 } errors ? string.Join(" ", errors) : $"Request failed ({(int)response.StatusCode}).");
            return new ApiResult<T>(false, default, message, (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Fail($"Could not reach the API — make sure it is running on :5080. ({ex.Message})");
        }
    }
}

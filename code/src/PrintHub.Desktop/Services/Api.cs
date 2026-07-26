using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace PrintHub.Desktop.Services;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public string[]? Errors { get; set; }
}

/// <summary>Thin HTTP client for the PrintHub API used by the desktop admin console.</summary>
public static class Api
{
    private static readonly HttpClient _http = new() { BaseAddress = new Uri("http://localhost:5080") };
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static string? Token { get; set; }
    public static string? Role { get; set; }
    public static string? UserName { get; set; }

    public static async Task<(bool ok, string? error, T? data)> SendAsync<T>(HttpMethod method, string path, object? body = null)
    {
        try
        {
            using var request = new HttpRequestMessage(method, path);
            if (body is not null) request.Content = JsonContent.Create(body, options: Json);
            if (!string.IsNullOrEmpty(Token)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            using var response = await _http.SendAsync(request);
            var payload = await response.Content.ReadFromJsonAsync<ApiResponse<T>>(Json);
            if (response.IsSuccessStatusCode) return (true, null, payload is null ? default : payload.Data);
            return (false, payload?.Message ?? $"Request failed ({(int)response.StatusCode}).", default);
        }
        catch (Exception ex)
        {
            return (false, "Cannot reach the API — make sure it is running on :5080. " + ex.Message, default);
        }
    }

    /// <summary>GET an endpoint that returns the DTO directly (not wrapped in ApiResponse), e.g. reports.</summary>
    public static async Task<(bool ok, string? error, T? data)> GetRaw<T>(string path)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            if (!string.IsNullOrEmpty(Token)) request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            using var response = await _http.SendAsync(request);
            if (!response.IsSuccessStatusCode) return (false, $"Request failed ({(int)response.StatusCode}).", default);
            return (true, null, await response.Content.ReadFromJsonAsync<T>(Json));
        }
        catch (Exception ex)
        {
            return (false, "Cannot reach the API — make sure it is running on :5080. " + ex.Message, default);
        }
    }

    public static Task<(bool, string?, T?)> Get<T>(string path) => SendAsync<T>(HttpMethod.Get, path);
    public static Task<(bool, string?, T?)> Post<T>(string path, object? body = null) => SendAsync<T>(HttpMethod.Post, path, body);
    public static Task<(bool, string?, T?)> Put<T>(string path, object? body = null) => SendAsync<T>(HttpMethod.Put, path, body);
    public static Task<(bool, string?, object?)> Delete(string path) => SendAsync<object>(HttpMethod.Delete, path);
}

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseAddress;
    private readonly string? _proxyPrefix;

    public ApiClient(string baseAddress, string? proxyPrefix = null)
    {
        _baseAddress = baseAddress.TrimEnd('/') + "/";
        _httpClient = new HttpClient();
        _proxyPrefix = string.IsNullOrWhiteSpace(proxyPrefix) ? null : proxyPrefix.TrimEnd('/');
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> GetDataAsync(string endpoint)
    {
        string targetUrl = _baseAddress + endpoint.TrimStart('/');
        string requestUrl = _proxyPrefix != null ? $"{_proxyPrefix}/{targetUrl}" : targetUrl;

        try
        {
            HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request to {requestUrl} failed {(int)response.StatusCode} {response.ReasonPhrase}. Body: {content}");
            }

            return content;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Error requesting {requestUrl}: {ex.Message}", ex);
        }
    }
}
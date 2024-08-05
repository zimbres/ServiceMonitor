namespace ServiceMonitor.Services;

public class HttpService
{
    private readonly ILogger<HttpService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private HttpClient _httpClient;

    public HttpService(ILogger<HttpService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> CheckHttpAsync(string url, string wordToCheck)
    {
        _httpClient = _httpClientFactory.CreateClient("Default");
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                if (wordToCheck is null || wordToCheck is "")
                {
                    _logger.LogInformation("HTTP request succeeded with status code {StatusCode}.", response.StatusCode);
                    return true;
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using var document = JsonDocument.Parse(content);
                    var json = document.RootElement.GetRawText();
                    var containsWord = json.Contains(wordToCheck);

                    if (containsWord)
                    {
                        _logger.LogInformation("HTTP request succeeded and response contains the word '{word}'.", wordToCheck);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("HTTP request succeeded but response does not contain the word '{word}'.", wordToCheck);
                        return false;
                    }
                }
            }
            else
            {
                _logger.LogWarning("HTTP request failed with status code {StatusCode}.", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception occurred while making HTTP request to {Url}. {ex}", url, ex.Message);
        }
        return false;
    }
}

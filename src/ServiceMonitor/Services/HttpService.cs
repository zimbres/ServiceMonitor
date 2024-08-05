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

    public async Task<bool> CheckHttpAsync(string url, bool checkForWord, string word = null)
    {
        _httpClient = _httpClientFactory.CreateClient("Default");
        try
        {
            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                if (!checkForWord)
                {
                    _logger.LogInformation("HTTP request succeeded with status code {StatusCode}.", response.StatusCode);
                    return true;
                }
                else if (word != null)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    using var document = JsonDocument.Parse(content);
                    var json = document.RootElement.GetRawText();
                    var containsWord = json.Contains(word);

                    if (containsWord)
                    {
                        _logger.LogInformation("HTTP request succeeded and contains the word '{word}'.", word);
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("HTTP request succeeded but does not contain the word '{word}'.", word);
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

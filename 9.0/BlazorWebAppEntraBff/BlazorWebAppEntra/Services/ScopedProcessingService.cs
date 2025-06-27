using System.Net.Http.Headers;
using Microsoft.Identity.Web;

namespace BlazorWebAppEntra.Services;

internal class ScopedProcessingService : IScopedProcessingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly ITokenAcquisition _tokenAcquisition;
    private int executionCount;

    public ScopedProcessingService(ILogger<ScopedProcessingService> logger, IHttpClientFactory httpClientFactory,
        ITokenAcquisition tokenAcquisition)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _tokenAcquisition = tokenAcquisition;
    }

    public async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            executionCount++;

            _logger.LogInformation(
                "Scoped Processing Service is working. Count: {Count}", executionCount);

            await GetWeatherAsync();

            await Task.Delay(20_000, stoppingToken);
        }
    }

    private async Task GetWeatherAsync()
    {
        // Using the client credentials flow
        var accessToken = await _tokenAcquisition.GetAccessTokenForAppAsync(
            scope: "api://{CLIENT ID}/.default",
            tenant: "{TENANT ID}"
        );

        // _logger.LogInformation("AccessToken: {}", accessToken);


        var httpClient = _httpClientFactory.CreateClient("testapi");

        using var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/weather-forecast");
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        using var response = await httpClient.SendAsync(requestMessage);

        var result = response.Content.ReadAsStringAsync().Result;
        _logger.LogInformation("Result: {}", result);
    }
}

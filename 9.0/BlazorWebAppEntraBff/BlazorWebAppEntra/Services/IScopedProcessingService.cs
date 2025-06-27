namespace BlazorWebAppEntra.Services;

internal interface IScopedProcessingService
{
    Task DoWorkAsync(CancellationToken stoppingToken);
}

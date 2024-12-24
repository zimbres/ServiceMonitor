namespace ServiceMonitor;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly TcpService _tcpService;
    private readonly HttpService _httpService;
    private readonly FileService _fileService;
    private readonly ControlService _controlService;
    private readonly Configuration _configuration;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, TcpService tcpService, HttpService httpService, ControlService controlService, FileService fileService)
    {
        _logger = logger;
        _configuration = configuration.GetSection(nameof(Configuration)).Get<Configuration>();
        _tcpService = tcpService;
        _httpService = httpService;
        _controlService = controlService;
        _fileService = fileService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogWarning("Service Monitor version: {version}", Assembly.GetExecutingAssembly().GetName().Version.ToString());

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var service in _configuration.Services)
            {
                await ProcessService(service);
            }

            await Task.Delay(_configuration.RunIntervalSeconds * 1000, stoppingToken);
        }
    }

    private async Task ProcessService(Service service)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Checking service: {service}...", service.ServiceName);
        }

        if (service.IpAddress is not null)
        {
            if (!_tcpService.CheckTcpPortListening(service.IpAddress, service.Port))
            {
                _controlService.RestartService(service);
            }
        }

        if (service.HttpUrl is not null)
        {
            if (!await _httpService.CheckHttpAsync(service.HttpUrl, service.WordToCheck))
            {
                _controlService.RestartService(service);
            }
        }

        if (service.FileToCheck is not null)
        {
            if (!_fileService.CheckFile(service.FileToCheck, service.FileCheckMinutes, service.FileMissingRestart))
            {
                _controlService.RestartService(service);
            }
        }
    }
}

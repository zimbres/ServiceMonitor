namespace ServiceMonitor;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly TcpService _tcpService;
    private readonly HttpService _httpService;
    private readonly ControlService _controlService;
    private readonly Configuration _configuration;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, TcpService tcpService, HttpService httpService, ControlService controlService)
    {
        _logger = logger;
        _configuration = configuration.GetSection(nameof(Configuration)).Get<Configuration>();
        _tcpService = tcpService;
        _httpService = httpService;
        _controlService = controlService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogWarning("Service Monitor version: {version}", Assembly.GetExecutingAssembly().GetName().Version.ToString());

        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Checking TCP port {IpAddress}:{Port}...", _configuration.IpAddress, _configuration.Port);
            }

            if (_configuration.CheckType == "Tcp")
            {
                if (!_tcpService.CheckTcpPortListening(_configuration.IpAddress, _configuration.Port))
                {
                    _logger.LogWarning("Port {Port} is not listening. Restarting service {ServiceName}...", _configuration.Port, _configuration.ServiceName);
                    _controlService.RestartService(_configuration.ServiceName);
                }
            }

            if (_configuration.CheckType == "Http")
            {
                if (!await _httpService.CheckHttpAsync(_configuration.HttpUrl, _configuration.CheckForWord, _configuration.Word))
                {
                    _logger.LogWarning("Port {Port} is not listening. Restarting service {ServiceName}...", _configuration.Port, _configuration.ServiceName);
                    _controlService.RestartService(_configuration.ServiceName);
                }
            }

            await Task.Delay(_configuration.RunIntervalSeconds * 1000, stoppingToken);
        }
    }
}

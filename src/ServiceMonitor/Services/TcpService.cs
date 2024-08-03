namespace ServiceMonitor.Services;

public class TcpService
{
    private readonly ILogger<TcpService> _logger;

    public TcpService(ILogger<TcpService> logger)
    {
        _logger = logger;
    }

    public bool CheckTcpPortListening(string ipAddress, int port)
    {
        try
        {
            using var tcpClient = new TcpClient();
            var result = tcpClient.BeginConnect(ipAddress, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            return success && tcpClient.Connected;
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception occurred while checking port. {ex}.", ex.Message);
            return false;
        }
    }
}

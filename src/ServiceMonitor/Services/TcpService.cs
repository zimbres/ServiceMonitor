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

            if (success && tcpClient.Connected)
            {
                return true;
            }
            else 
            {
                _logger.LogWarning("Port {Port} is not listening on {ipAddress}.", port, ipAddress);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception occurred while checking port. {ex}.", ex.Message);
            return false;
        }
    }
}

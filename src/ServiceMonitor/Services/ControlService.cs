namespace ServiceMonitor.Services;

public class ControlService
{
    private readonly ILogger<ControlService> _logger;

    public ControlService(ILogger<ControlService> logger)
    {
        _logger = logger;
    }

    public void RestartService(string serviceName)
    {
        try
        {
            using var serviceController = new ServiceController(serviceName);
            if (serviceController.Status == ServiceControllerStatus.Stopped)
            {
                _logger.LogInformation("Starting service {serviceName}...", serviceName);
                serviceController.Start();
            }
            
            else if (serviceController.Status == ServiceControllerStatus.Running)
            {
                _logger.LogInformation("Restarting service {serviceName}...", serviceName);
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));
                serviceController.Start();
            }
            
            if (serviceController.Status == ServiceControllerStatus.StopPending)
            {
                _logger.LogInformation("Waiting service {serviceName} to stop...", serviceName);
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));
                _logger.LogInformation("Starting service {serviceName}...", serviceName);
                serviceController.Start();
            }
            serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(60));
        }
        catch (Exception ex)
        {
            _logger.LogError("Exception occurred while restarting service. {ex}", ex.Message);
        }
    }
}

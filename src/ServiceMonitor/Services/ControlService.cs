namespace ServiceMonitor.Services;

public class ControlService
{
    private readonly ILogger<ControlService> _logger;

    public ControlService(ILogger<ControlService> logger)
    {
        _logger = logger;
    }

    public void RestartService(Service service)
    {
        try
        {
            if (service.CustomCommand != null)
            {
                using PowerShell powerShell = PowerShell.Create();
                powerShell.AddScript(service.CustomCommand);
                var processes = powerShell.Invoke();
                _logger.LogWarning("Custom command '{service.CustomCommand}' executed.", service.CustomCommand);
                return;
            }

            using var serviceController = new ServiceController(service.ServiceName);
            if (serviceController.Status == ServiceControllerStatus.Stopped)
            {
                _logger.LogWarning("Starting service {serviceName}...", service.ServiceName);
                serviceController.Start();
            }

            else if (serviceController.Status == ServiceControllerStatus.Running)
            {
                _logger.LogWarning("Restarting service {serviceName}...", service.ServiceName);
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));
                serviceController.Start();
            }

            if (serviceController.Status == ServiceControllerStatus.StopPending)
            {
                _logger.LogWarning("Waiting service {serviceName} to stop...", service.ServiceName);
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(60));
                _logger.LogWarning("Starting service {serviceName}...", service.ServiceName);
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

namespace ServiceMonitor.Configurations;

internal class Configuration
{
    public int RunIntervalSeconds { get; set; }
    public List<Service> Services { get; set; }
}

public class Service
{
    public string ServiceName { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public string HttpUrl { get; set; }
    public string WordToCheck { get; set; }
    public string CustomCommand { get; set; }
    public string FileToCheck { get; set; }
    public int FileCheckMinutes { get; set; }
    public bool FileMissingRestart { get; set; }
}

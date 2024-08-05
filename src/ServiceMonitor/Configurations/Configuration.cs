namespace ServiceMonitor.Configurations;

internal class Configuration
{
    public int RunIntervalSeconds { get; set; }
    public List<Service> Services { get; set; }
}

internal class Service
{
    public string ServiceName { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public string HttpUrl { get; set; }
    public string WordToCheck { get; set; }
}

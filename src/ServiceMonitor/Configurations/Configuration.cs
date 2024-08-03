namespace ServiceMonitor.Configurations;

internal class Configuration
{
    public int RunIntervalSeconds { get; set; }
    public string ServiceName { get; set; }
    public string IpAddress { get; set; }
    public int Port { get; set; }
    public string HttpUrl { get; set; }
    public bool CheckForWord { get; set; }
    public string Word { get; set; }
    public string CheckType { get; set; }
}

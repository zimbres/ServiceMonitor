# Windows Service Monitor

This application runs as a Windows Service and is designed to monitor and manage the health of a specified service. It performs this task by either probing a TCP port to ensure it is listening or by making an HTTP request to check for a successful status code or the presence of a specific word in the response. If the monitored service is found to be unhealthy, the application will automatically restart it.

### Configuration required:

```json

{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.Hosting.Lifetime": "Warning"
    }
  },
  "Configuration": {
    "RunIntervalSeconds": 30,
    "ServiceName": "YourServiceName",
    "IpAddress": "127.0.0.1",
    "Port": 8080,
    "HttpUrl": "http://localhost:8080/api/health",
    "CheckForWord": false,
    "Word": "",
    "CheckType" : "Tcp"
  }
}

```



---

[Microsoft Help: Create Windows Service](https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service)

---

### Requires .Net Runtime 8.0

[Download .NET](https://dotnet.microsoft.com/en-us/download)

---

Application icon by [Flaticon](https://www.flaticon.com/br/icones-gratis/marketing-de-midia-social)
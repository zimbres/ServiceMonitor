var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "Service Monitor";
});
builder.Services.AddHttpClient("Default")
.ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
    };
});
builder.Services.AddSingleton<TcpService>();
builder.Services.AddSingleton<HttpService>();
builder.Services.AddSingleton<ControlService>();

builder.Services.AddHostedService<Worker>().Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

var host = builder.Build();
host.Run();

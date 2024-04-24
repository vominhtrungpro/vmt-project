using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = new HostBuilder();

builder.ConfigureServices((context, services) =>
{
    services.AddHostedService<Consumer>();
});

var host = builder.Build();
using (host)
{
    await host.RunAsync();
}
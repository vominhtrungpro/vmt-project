using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using vmt_project.consumer;

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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using vmt_project.dal.Contracts;
using vmt_project.dal.Implementations;
using vmt_project.dal.Models.Context;
using vmt_project.search;


var builder = new HostBuilder();

builder.ConfigureAppConfiguration(cfg =>
{
    cfg.AddJsonFile("appsettings.json");
});

builder.ConfigureServices((context, services) =>
{
    var configuration = context.Configuration;
    var config = new Config(configuration);
    config.SetConfig();


    var sqlc = Environment.GetEnvironmentVariable("ConnectionString");

    services.AddDbContext<VmtDbContext>(options =>
    {
        options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionString"));
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });
    services.AddHostedService<Search>();


});


var host = builder.Build();
using (host)
{
    await host.RunAsync();
}
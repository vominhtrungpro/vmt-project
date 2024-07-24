using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using vmt_project.cronjob_search;
using vmt_project.dal.Contracts;
using vmt_project.dal.Implementations;
using vmt_project.dal.Models.Context;
using vmt_project.dal.Models.Entities;
using vmt_project.services.Contracts;
using vmt_project.services.Implementations;

var builder = new HostBuilder();

builder.ConfigureServices((context, services) =>
{
    var configuration = context.Configuration;

    var sqlc = Environment.GetEnvironmentVariable("ConnectionString");
    if (sqlc == null)
    {
        sqlc = "Server=tcp:vmt-database.database.windows.net,1433;Initial Catalog=vmt-database;Persist Security Info=False;User ID=CloudSA25e3be20;Password=Tin14091998;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
    }

    services.AddDbContext<VmtDbContext>(options =>
    {
        options.UseSqlServer(sqlc);
        options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    });
    services.AddHostedService<Worker>();

    services.AddScoped<ICronjobService, CronjobService>();

    services.AddScoped<ICharacterRepository, CharacterRepository>();

    services.AddIdentity<User, Role>(o =>
    {
        o.Password.RequireDigit = false;
        o.Password.RequireLowercase = false;
        o.Password.RequireUppercase = false;
        o.Password.RequireNonAlphanumeric = false;
        o.User.RequireUniqueEmail = true;
    })
.AddEntityFrameworkStores<VmtDbContext>()
.AddDefaultTokenProviders();
});

var host = builder.Build();

using (host)
{
    await host.RunAsync();
}
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedisCache.Core;
using vmt_project.dal.Contracts;
using vmt_project.dal.Implementations;
using vmt_project.dal.Models.Context;
using vmt_project.dal.Models.Entities;
using vmt_project.kafka_consumer.StartUp;
using vmt_project.services.Contracts;
using vmt_project.services.Implementations;
using vmt_project.services.Kafka;
using vmt_project.services.Kafka.Base;
using vmt_project.services.Redis;
using vmt_project.services.Redis.Base;

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
    services.AddHostedService<Consumer>();

    //services.AddIdentity<User,Role>(o => {
    //    o.Password.RequireDigit = false;
    //    o.Password.RequireLowercase = false;
    //    o.Password.RequireUppercase = false;
    //    o.Password.RequireNonAlphanumeric = false;
    //    o.User.RequireUniqueEmail = true;
    //})
    //.AddEntityFrameworkStores<VmtDbContext>()
    //.AddDefaultTokenProviders();

    //services.AddRedisCache(options =>
    //{
    //    options.Configuration = Environment.GetEnvironmentVariable("RedisConnectionString");
    //    options.InstanceName = Environment.GetEnvironmentVariable("RedisInstanceName");
    //});

    //services.AddScoped<IUserInfoService, UserInfoService>();
    //services.AddScoped<IUserInfoRepository, UserInfoRepository>();
    //services.AddScoped<IUserRedisService, UserRedisService>();
    //services.AddScoped<IGenericRedisService, GenericRedisService>();
    //services.AddScoped<IUserInfoKafkaService, UserInfoKafkaService>();
    //services.AddScoped<IGenericKafkaService, GenericKafkaService>();

    services.AddScoped<IKafkaService, KafkaService>();
    services.AddScoped<IUserInfoRepository, UserInfoRepository>();

});


var host = builder.Build();
using (host)
{
    await host.RunAsync();
}
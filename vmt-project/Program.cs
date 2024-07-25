using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using RedisCache.Core;
using Serilog;
using StackExchange.Redis;
using System.Text;
using vmt_project.dal.Models.Context;
using vmt_project.dal.Models.Entities;
using vmt_project.Hubs;
using vmt_project.services.Elastic;
using vmt_project.StartUp;

var builder = WebApplication.CreateBuilder(args);

new Config(builder.Configuration).SetConfig();

new ServiceRepoMapping().Mapping(builder);

new ServiceRepoMapping().Init(builder);

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<MessageHub>("/message");

app.Run();

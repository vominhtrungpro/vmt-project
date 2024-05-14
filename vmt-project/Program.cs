using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RedisCache.Core;
using StackExchange.Redis;
using System.Text;
using vmt_project.dal.Models.Context;
using vmt_project.dal.Models.Entities;
using vmt_project.Hubs;
using vmt_project.StartUp;

var builder = WebApplication.CreateBuilder(args);

new Config(builder.Configuration).SetConfig();

string sql_conn = Environment.GetEnvironmentVariable("ConnectionString");

// Add services to the container.
builder.Services.AddDbContext<VmtDbContext>(options =>
{
    options.UseSqlServer(sql_conn);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddRedisCache(options =>
{
    options.Configuration = Environment.GetEnvironmentVariable("RedisConnectionString");
    options.InstanceName = Environment.GetEnvironmentVariable("RedisInstanceName");
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Vmt API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Please enter a valid token.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    opt.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("ValidIssuer"),
        ValidAudience = Environment.GetEnvironmentVariable("ValidAudience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("Secret")))
    };
});
builder.Services.AddAuthorization();

builder.Services.AddIdentity<User, vmt_project.dal.Models.Entities.Role>(o =>
{
    o.Password.RequireDigit = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireNonAlphanumeric = false;
    o.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<VmtDbContext>()
.AddDefaultTokenProviders();

new ServiceRepoMapping().Mapping(builder);

builder.Services.AddCors(options => options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod()
                   .SetIsOriginAllowed((host) => true)
                   .AllowCredentials();
        }));

var sign_conn = Environment.GetEnvironmentVariable("SignalrConnectionString");
if (sign_conn.IsNullOrEmpty())
{
    builder.Services.AddSignalR();
}
else 
{
    builder.Services.AddSignalR().AddAzureSignalR(sign_conn);
}

var app = builder.Build();

app.UseCors(builder =>
{
    builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("CorsPolicy");

app.MapHub<MessageHub>("/message");

app.Run();

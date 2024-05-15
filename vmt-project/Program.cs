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

var sql_conn = Environment.GetEnvironmentVariable("ConnectionString");

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

var sign_conn = Environment.GetEnvironmentVariable("SignalrConnectionString");

if (sign_conn == "none" || sign_conn.IsNullOrEmpty())
{
    builder.Services.AddSignalR();
}
else
{
    builder.Services.AddSignalR().AddAzureSignalR(options => { 
        options.ConnectionString = sign_conn;
    });
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000", "https://white-bush-0e7e7d600.5.azurestaticapps.net")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});


var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<MessageHub>("/message");

app.Run();

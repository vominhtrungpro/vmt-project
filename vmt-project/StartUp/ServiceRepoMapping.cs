using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nest;
using RedisCache.Core;
using System.Text;
using vmt_project.dal.Contracts;
using vmt_project.dal.Implementations;
using vmt_project.dal.Models.Context;
using vmt_project.dal.Models.Entities;
using vmt_project.Hubs;
using vmt_project.services.Contracts;
using vmt_project.services.Elastic;
using vmt_project.services.Elastic.Base;
using vmt_project.services.Implementations;
using vmt_project.services.Kafka;
using vmt_project.services.Kafka.Base;
using vmt_project.services.Redis;
using vmt_project.services.Redis.Base;

namespace vmt_project.StartUp
{
    public class ServiceRepoMapping
    {
        public ServiceRepoMapping()
        {
            
        }
        public void Mapping(WebApplicationBuilder builder)
        {
            #region Service Mapping
            builder.Services.AddScoped<ICharacterService, CharacterService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IConfigurationService, ConfigurationService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUploadService, UploadService>();
            builder.Services.AddScoped<IUserInfoService, UserInfoService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IMyProfileService, MyProfileService>();
            #endregion Service Mapping

            #region Redis Cahe Service Mapping
            builder.Services.AddScoped<IUserRedisService, UserRedisService>();
            builder.Services.AddScoped<IGenericRedisService, GenericRedisService>();
            #endregion Redis Cahe Service Mapping

            #region Kafka Service Mapping
            builder.Services.AddScoped<IUserInfoKafkaService, UserInfoKafkaService>();
            builder.Services.AddScoped<IGenericKafkaService, GenericKafkaService>();
            #endregion Kafka Service Mapping

            #region Repository Mapping
            builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
            builder.Services.AddScoped<IUserInfoRepository, UserInfoRepository>();
            builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddScoped<IMyProfileRepository, MyProfileRepository>();
            builder.Services.AddScoped<IMyProfilePictureRepository, MyProfilePictureRepository>();
            #endregion Repository Mapping

            #region Elastic Service Mapping
            builder.Services.AddScoped<ICharacterElasticService, CharacterElasticService>();
            builder.Services.AddScoped<IGenericElasticService<Character>, GenericElasticService<Character>>();
            #endregion Elastic Service Mapping
        }

        public void Init(WebApplicationBuilder builder)
        {
            #region Signalr 
            var sign_conn = Environment.GetEnvironmentVariable("SignalrConnectionString");

            if (sign_conn == "none" || String.IsNullOrEmpty(sign_conn))
            {
                builder.Services.AddSignalR();
            }
            else
            {
                builder.Services.AddSignalR().AddAzureSignalR(options =>
                {
                    options.ConnectionString = sign_conn;
                });
            }
            #endregion Signalr 

            #region CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:3000", "https://white-bush-0e7e7d600.5.azurestaticapps.net")
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      .AllowCredentials());
            });
            #endregion CORS

            #region Elastic
            var settings = new ConnectionSettings(new Uri("https://c1e26596e2114447af4c0f0224e4a5de.eastus2.azure.elastic-cloud.com/"))
                .DefaultIndex("character-index")
                .BasicAuthentication("elastic", "LmKZuwze4VkJotLKRTtqbo2l")
                .EnableApiVersioningHeader();

            var client = new ElasticClient(settings);

            builder.Services.AddSingleton(client);
            #endregion Elastic

            #region Elastic
            var sql_conn = Environment.GetEnvironmentVariable("ConnectionString");

            builder.Services.AddDbContext<VmtDbContext>(options =>
            {
                options.UseSqlServer(sql_conn);
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            });
            #endregion Elastic

            #region Redis
            builder.Services.AddRedisCache(options =>
            {
                options.Configuration = Environment.GetEnvironmentVariable("RedisConnectionString");
                options.InstanceName = Environment.GetEnvironmentVariable("RedisInstanceName");
            });
            #endregion Redis

            #region Swagger
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
                        },new string[] {}
                    }
                });
            });
            #endregion Swagger

            #region Authentication
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
            #endregion Authentication

            #region Identity
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
            #endregion Identity

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddAuthorization();
        }
    }
}

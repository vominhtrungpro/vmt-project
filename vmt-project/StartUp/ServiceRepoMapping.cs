using vmt_project.dal.Contracts;
using vmt_project.dal.Implementations;
using vmt_project.services.Contracts;
using vmt_project.services.Implementations;
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
            #endregion Service Mapping

            #region Redis Cahe Service Mapping
            builder.Services.AddScoped<IUserRedisService, UserRedisService>();
            builder.Services.AddScoped<IGenericRedisService, GenericRedisService>();
            #endregion Redis Cahe Service Mapping

            #region Repository Mapping
            builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
            #endregion Repository Mapping
        }
    }
}

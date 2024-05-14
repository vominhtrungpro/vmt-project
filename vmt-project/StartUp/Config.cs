using NetCore.Infrastructure.Common.Helpers;
using vmt_project.dal.Contracts;
using vmt_project.dal.Implementations;
using vmt_project.services.Contracts;
using vmt_project.services.Implementations;

namespace vmt_project.StartUp
{
    public class Config
    {
        private readonly IConfiguration _configuration;
        public Config(IConfiguration configuration)
        {

            _configuration = configuration;

        }
        public void SetConfig()
        {
            var jwtConfig = _configuration.GetSection("JwtConfig");
            GetAndSetConfig("ValidIssuer", jwtConfig);
            GetAndSetConfig("ValidAudience", jwtConfig);
            GetAndSetConfig("Secret", jwtConfig);
            GetAndSetConfig("AccessTokenExpiresIn", jwtConfig);
            GetAndSetConfig("RefreshTokenExpiresIn", jwtConfig);

            var connectionStrings = _configuration.GetSection("ConnectionStrings");
            GetAndSetConfig("ConnectionString", connectionStrings);

            var email = _configuration.GetSection("Email");
            GetAndSetConfig("EmailUsername", email);
            GetAndSetConfig("EmailPassword", email);
            GetAndSetConfig("From", email);

            var redis = _configuration.GetSection("Redis");
            GetAndSetConfig("RedisConnectionString", redis);
            GetAndSetConfig("RedisInstanceName", redis);
            GetAndSetConfig("RedisCacheTimeExpire", redis);

            var storage = _configuration.GetSection("Storage");
            GetAndSetConfig("StorageConnectionString", storage);
            GetAndSetConfig("StorageImageContainerName", storage);
            GetAndSetConfig("StorageBlobUrl", storage);

            var kafka = _configuration.GetSection("Kafka");
            GetAndSetConfig("KafkaBootstrapServers", kafka);
            GetAndSetConfig("KafkaIsUsing", kafka);

            var signalr = _configuration.GetSection("Signalr");
            GetAndSetConfig("SignalrConnectionString",signalr);
        }
        private void GetAndSetConfig(string config,IConfigurationSection section)
        {
            var value = Environment.GetEnvironmentVariable(config);
            if (value.IsNullOrEmpty()) 
            {
                Environment.SetEnvironmentVariable(config, section[config]);
            }
        }
    }
}

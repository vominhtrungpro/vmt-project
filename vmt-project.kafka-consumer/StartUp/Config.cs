using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.kafka_consumer.StartUp
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
        }
        private void GetAndSetConfig(string config, IConfigurationSection section)
        {
            var value = Environment.GetEnvironmentVariable(config);
            if (value.IsNullOrEmpty())
            {
                Environment.SetEnvironmentVariable(config, section[config]);
            }
        }
    }
}

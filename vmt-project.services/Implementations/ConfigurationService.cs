using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.models.Model.Config;
using vmt_project.services.Contracts;

namespace vmt_project.services.Implementations
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfiguration _configuration;
        public ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public JwtConfig GetJwtConfig()
        {
            var jwtSettings = _configuration.GetSection("JwtConfig");
            var accessTokenExpiresIn = Environment.GetEnvironmentVariable("AccessTokenExpiresIn");
            if (string.IsNullOrEmpty(accessTokenExpiresIn))
            {
                accessTokenExpiresIn = jwtSettings["AccessTokenExpiresIn"];
            }
            var refreshTokenExpiresIn = Environment.GetEnvironmentVariable("RefreshTokenExpiresIn");
            if (string.IsNullOrEmpty(refreshTokenExpiresIn))
            {
                refreshTokenExpiresIn = jwtSettings["RefreshTokenExpiresIn"];
            }
            var secret = Environment.GetEnvironmentVariable("Secret");
            if (string.IsNullOrEmpty(secret))
            {
                secret = jwtSettings["Secret"];
            }
            var validIssuer = Environment.GetEnvironmentVariable("ValidIssuer");
            if (string.IsNullOrEmpty(validIssuer))
            {
                validIssuer = jwtSettings["ValidIssuer"];
            }
            var validAudience = Environment.GetEnvironmentVariable("ValidAudience");
            if (string.IsNullOrEmpty(validAudience))
            {
                validAudience = jwtSettings["ValidAudience"];
            }
            return new JwtConfig
            {
                AccessTokenExpiresIn = Convert.ToDouble(accessTokenExpiresIn),
                RefreshTokenExpiresIn = Convert.ToDouble(refreshTokenExpiresIn),
                Secret = secret,
                ValidAudience = validAudience,
                ValidIssuer = validIssuer
            };
        }

    }
}

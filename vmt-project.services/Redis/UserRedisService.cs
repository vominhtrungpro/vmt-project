using Azure.Core;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.models.DTO.User;
using vmt_project.services.Redis.Base;

namespace vmt_project.services.Redis
{
    public class UserRedisService : IUserRedisService
    {
        private readonly IGenericRedisService _redis;
        private readonly int _cacheTimeExpire;
        public UserRedisService(IGenericRedisService redis)
        {
            _redis = redis;
            _cacheTimeExpire = int.Parse(Environment.GetEnvironmentVariable("RedisCacheTimeExpire"));
        }
        private string Key(string userId)
        {
            return "user-profile:" + userId;
        }
        public async Task SetUserProfileCache(UserDto user)
        {
            try
            {
                await _redis.SetAsync(Key(user.Id), user, _cacheTimeExpire);
            }
            catch (Exception)
            {
            }
            
        }
        public async Task<UserDto> GetUserProfileCache(string userId)
        {
            try
            {
                return await _redis.GetAsync<UserDto>(Key(userId));
            }
            catch (Exception)
            {
                return null;
            }
            
        }

    }
}

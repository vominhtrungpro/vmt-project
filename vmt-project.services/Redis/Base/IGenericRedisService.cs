using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.services.Redis.Base
{
    public interface IGenericRedisService
    {
        T Get<T>(string key);
        Task<T> GetAsync<T>(string key);
        void Set(string key, object data, int cacheTimeInMinutes);
        Task SetAsync(string key, object data, int cacheTimeInMinutes);
        T GetOrSet<T>(string key, Func<T> factory, int cacheTimeInMinutes);
        void Remove(string key);
        Task RemoveAsync(string key);
        bool TryGetValue<T>(string key, out T result);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;

namespace vmt_project.services.Redis
{
    public interface IUserRedisService
    {
        Task SetUserProfileCache(User user);
        Task<User> GetUserProfileCache(string userId);
    }
}

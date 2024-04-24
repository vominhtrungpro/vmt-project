using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.models.DTO.User;

namespace vmt_project.services.Redis
{
    public interface IUserRedisService
    {
        Task SetUserProfileCache(UserDto user);
        Task<UserDto> GetUserProfileCache(string userId);
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.common.Helpers;
using vmt_project.dal.Contracts;
using vmt_project.dal.Models.Entities;
using vmt_project.models.DTO.User;
using vmt_project.models.DTO.UserInfo;
using vmt_project.models.Request.Kafka;
using vmt_project.models.Request.UserInfo;
using vmt_project.services.Contracts;
using vmt_project.services.Kafka;
using vmt_project.services.Redis;

namespace vmt_project.services.Implementations
{
    public class UserInfoService : IUserInfoService
    {
        public readonly IUserInfoRepository _userInfoRepository;
        private readonly UserManager<User> _userManager;
        private readonly IUserRedisService _userRedisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserInfoKafkaService _userInfoKafkaService;
        public UserInfoService(IUserInfoRepository userInfoRepository,UserManager<User> userManager,IUserRedisService userRedisService, IHttpContextAccessor httpContextAccessor,IUserInfoKafkaService userInfoKafkaService)
        {
            _userInfoRepository = userInfoRepository;
            _userManager = userManager;
            _userRedisService = userRedisService;
            _httpContextAccessor = httpContextAccessor;
            _userInfoKafkaService = userInfoKafkaService;
        }
        public async Task<AppActionResult> Upsert(CreateUserInfoRequest request)
        {
            var result = new AppActionResult();
            var userEntity = await _userManager.FindByIdAsync(request.UserId);
            if (userEntity is null)
            {
                return result.BuildError("User not found!");
            }
            var userInfoEntity = _userInfoRepository.FindBy(m => m.UserId == request.UserId).FirstOrDefault();
            if (userInfoEntity != null)
            {
                var message = new UpdateUserInfoRequestMessage()
                {
                    AvatarUrl = request.AvatarUrl,
                    UserId = request.UserId,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    CurrentUserId = ClaimHelper.GetCurrentUserId(_httpContextAccessor)
                };
                _userInfoKafkaService.PublishUpdateUserInfo(message);
                var userDtoCache = await _userRedisService.GetUserProfileCache(request.UserId);
                if (userDtoCache != null)
                {
                    if (userDtoCache.UserInfo != null)
                    {
                        userDtoCache.UserInfo.AvatarUrl = request.AvatarUrl;
                        userDtoCache.UserInfo.FirstName = request.FirstName;
                        userDtoCache.UserInfo.LastName = request.LastName;
                    }
                    else
                    {
                        userDtoCache.UserInfo = new UserInfoDto()
                        {
                            AvatarUrl = request.AvatarUrl,
                            FirstName = request.FirstName,
                            LastName= request.LastName,
                        };
                    }
                    await _userRedisService.SetUserProfileCache(userDtoCache);
                }
                return result.BuildResult("Success");
            }
            else
            {
                var message = new InsertUserInfoRequestMessage()
                {
                    AvatarUrl = request.AvatarUrl,
                    UserId = request.UserId,
                    CurrentUserId = ClaimHelper.GetCurrentUserId(_httpContextAccessor),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                };
                _userInfoKafkaService.PublishInsertUserInfo(message);
                var userDtoCache = await _userRedisService.GetUserProfileCache(request.UserId);
                if (userDtoCache != null)
                {
                    if (userDtoCache.UserInfo != null)
                    {
                        userDtoCache.UserInfo.AvatarUrl = request.AvatarUrl;
                        userDtoCache.UserInfo.FirstName = request.FirstName;
                        userDtoCache.UserInfo.LastName = request.LastName;
                    }
                    else
                    {
                        userDtoCache.UserInfo = new UserInfoDto()
                        {
                            AvatarUrl = request.AvatarUrl,
                            FirstName = request.FirstName,
                            LastName = request.LastName,
                        };
                    }
                    await _userRedisService.SetUserProfileCache(userDtoCache);
                }
                return result.BuildResult("Success");
            }
        }
    }
}

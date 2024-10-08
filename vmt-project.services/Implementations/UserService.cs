﻿using Azure.Core;
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
using vmt_project.dal.Models.Entities;
using vmt_project.models.DTO.User;
using vmt_project.models.Request.User;
using vmt_project.services.Contracts;
using vmt_project.services.Redis;

namespace vmt_project.services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IUserRedisService _userRedisService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(UserManager<User> userManager,IEmailService emailService, IUserRedisService userRedisService, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _emailService = emailService;
            _userRedisService = userRedisService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<AppActionResultData<UserDto>> GetUserProfileById(string id)
        {
            var result = new AppActionResultData<UserDto>();
            var userDtoCache = await _userRedisService.GetUserProfileCache(id);
            if (userDtoCache != null)
            {
                await _userRedisService.SetUserProfileCache(userDtoCache);
                return result.BuildResult(userDtoCache);
            }
            var userEntity = await _userManager.FindByIdAsync(id);
            if (userEntity == null)
            {
                return result.BuildError("User not found!");
            }
            var userDto = new UserDto()
            {
                Id = userEntity.Id,
                UserName = userEntity.UserName,
                Email = userEntity.Email,
            };
            if (userEntity.UserInfo != null)
            {
                userDto.UserInfo = new models.DTO.UserInfo.UserInfoDto()
                {
                    AvatarUrl = userEntity.UserInfo.AvatarUrl,
                    FirstName = userEntity.UserInfo.FirstName,
                    LastName = userEntity.UserInfo.LastName,
                };
            }
            await _userRedisService.SetUserProfileCache(userDto);
            return result.BuildResult(userDto);
        }
        public async Task<AppActionResult> ChangePasswordUserAsync(ChangePasswordRequest request)
        {
            var result = new AppActionResult();
            var userEntity = await _userManager.FindByIdAsync(request.UserId);
            if (userEntity == null)
            {
                return result.BuildError("User not found!");
            }
            var currentPass = await _userManager.CheckPasswordAsync(userEntity, request.CurrentPassword);
            if (!currentPass)
            {
                return result.BuildError("Wrong current password!");
            }
            if (request.Password != request.PasswordConfirm)
            {
                return result.BuildError("Password doesn't match!");
            }
            var editPasswordResult = await _userManager.ChangePasswordAsync(userEntity, request.CurrentPassword, request.Password);
            if ( editPasswordResult.Succeeded)
            {
                return result.BuildResult("Success!");
            }
            return result.BuildError("Cant change pass!"); ;
        }
        public async Task<AppActionResult> ForgetPassword(ForgetPasswordRequest request)
        {
            var result = new AppActionResult();
            var userEntity = await _userManager.FindByEmailAsync(request.Email);
            if (userEntity != null)
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(userEntity);
                await _emailService.SendEmailForgetPassword(userEntity, resetToken);
                return result.BuildResult("Success!");
            }
            else
            {
                return result.BuildError("User not found!"); ;
            }
        }
        public async Task<AppActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var result = new AppActionResult();
            var userEntity = await _userManager.FindByIdAsync(request.UserId);
            if (userEntity == null)
            {
                return result.BuildError("User not found!");
            }
            var editPasswordResult = await _userManager.ResetPasswordAsync(userEntity, request.ResetToken, request.NewPassword);
            if (editPasswordResult.Succeeded)
            {
                return result.BuildResult("Password reset success!");
            }
            return result.BuildError("Cant reset pass!"); ;
        }
    }
}

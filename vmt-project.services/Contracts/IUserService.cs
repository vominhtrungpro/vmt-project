using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;
using vmt_project.models.DTO.User;
using vmt_project.models.Request.User;

namespace vmt_project.services.Contracts
{
    public interface IUserService
    {
        Task<AppActionResultData<UserDto>> GetUserProfileById(string id);
        Task<AppActionResult> ChangePasswordUserAsync(ChangePasswordRequest request);
        Task<AppActionResult> ForgetPassword(ForgetPasswordRequest request);
        Task<AppActionResult> ResetPassword(ResetPasswordRequest request);
    }
}

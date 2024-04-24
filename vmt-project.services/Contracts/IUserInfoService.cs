using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.models.DTO.UserInfo;
using vmt_project.models.Request.UserInfo;

namespace vmt_project.services.Contracts
{
    public interface IUserInfoService
    {
        Task<AppActionResult> Upsert(CreateUserInfoRequest request);
    }
}

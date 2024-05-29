using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.models.DTO.MyProfile.cs;
using vmt_project.models.Request.MyProfile;

namespace vmt_project.services.Contracts
{
    public interface IMyProfileService
    {
        Task<AppActionResult> Create(CreateMyProfileRequest request);
        Task<AppActionResult> Update(UpdateMyProfileRequest request);
        Task<AppActionResultData<List<MyProfileDto>>> List();
    }
}

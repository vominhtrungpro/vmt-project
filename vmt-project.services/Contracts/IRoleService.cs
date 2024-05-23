using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.models.DTO.Role;
using vmt_project.models.Request.Role;

namespace vmt_project.services.Contracts
{
    public interface IRoleService
    {
        Task<AppActionResultData<List<RoleDto>>> ListRole();
        Task<AppActionResult> CreateRole(CreateRoleRequest request);
        Task<AppActionResult> SetRole(SetRoleRequest request);
    }
}

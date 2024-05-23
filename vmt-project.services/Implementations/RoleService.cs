using Microsoft.AspNetCore.Identity;
using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Contracts;
using vmt_project.dal.Models.Entities;
using vmt_project.models.DTO.Role;
using vmt_project.models.Request.Role;
using vmt_project.services.Contracts;

namespace vmt_project.services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly IUserRoleRepository _userRoleRepository;
        public RoleService(RoleManager<Role> roleManager,IUserRoleRepository userRoleRepository)
        {
            _roleManager = roleManager;
            _userRoleRepository = userRoleRepository;
        }
        public async Task<AppActionResult> SetRole(SetRoleRequest request)
        {
            var result = new AppActionResult();
            try
            {
                _userRoleRepository.Create(request.UserId, request.RoleId);
                return result.BuildResult("Success");
            }
            catch (Exception)
            {
                return result.BuildError("Error");
            }
        }
        public async Task<AppActionResultData<List<RoleDto>>> ListRole()
        {
            var result = new AppActionResultData<List<RoleDto>>();
            try
            {
                var roles = _roleManager.Roles.Select(role => new RoleDto 
                { 
                    Id = role.Id,
                    Name = role.Name
                }).ToList();
                return result.BuildResult(roles);

            }
            catch (Exception)
            {
                return result.BuildError("Error");
            }
        }
        public async Task<AppActionResult> CreateRole(CreateRoleRequest request)
        {
            var result = new AppActionResult();
            try
            {
                var checkExist = _roleManager.FindByNameAsync(request.RoleName);
                if (checkExist != null) 
                {
                    return result.BuildError("Role exist!");
                }
                var roleEntity = new Role()
                {
                    Name = request.RoleName,
                };
                await _roleManager.CreateAsync(roleEntity);
                return result.BuildResult("Success");

            }
            catch (Exception)
            {
                return result.BuildError("Error");
            }
        }

    }
}

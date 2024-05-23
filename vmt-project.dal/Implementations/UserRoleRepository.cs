using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Base;
using vmt_project.dal.Contracts;
using vmt_project.dal.Models.Context;
using vmt_project.dal.Models.Entities;

namespace vmt_project.dal.Implementations
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly VmtDbContext _dbContext;
        public UserRoleRepository(VmtDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Create(string userId,string roleId)
        {
            _dbContext.UserRoles.Add(new UserRoleMapping { UserId = userId, RoleId = roleId });
            _dbContext.SaveChanges();
        }
    }
}

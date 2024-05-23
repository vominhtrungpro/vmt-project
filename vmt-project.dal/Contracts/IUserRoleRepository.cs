using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.dal.Contracts
{
    public interface IUserRoleRepository
    {
        void Create(string userId, string roleId);
    }
}

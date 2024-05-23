using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.models.Request.Role
{
    public class SetRoleRequest
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}

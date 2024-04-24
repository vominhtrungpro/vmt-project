using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.dal.Models.Entities
{
    public class UserInfo : Entity
    {
        public string? AvatarUrl { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}

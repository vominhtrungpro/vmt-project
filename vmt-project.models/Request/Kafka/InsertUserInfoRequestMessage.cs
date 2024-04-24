using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.dal.Models.Entities;

namespace vmt_project.models.Request.Kafka
{
    public class InsertUserInfoRequestMessage
    {
        public string? AvatarUrl { get; set; }
        public string UserId { get; set; }
        public string CurrentUserId { get; set; }
    }
}

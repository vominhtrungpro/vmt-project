using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.models.Request.Signalr
{
    public class HubMessage
    {
        public string? Username { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Content { get; set; }
    }
}

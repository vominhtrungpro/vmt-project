using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.models.Request.Character
{
    public class CallBackRequest
    {
        public string id { get; set; }
        public string type { get; set; }
        public string messageid { get; set; }
        public string status { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string message_type { get; set; }
        public string message_text { get; set; }
        public string message_media { get; set; }
    }
}

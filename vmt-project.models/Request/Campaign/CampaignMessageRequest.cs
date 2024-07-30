using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.models.Request.Campaign
{
    public class CampaignMessageRequest
    {
        public Guid Id { get; set; }

        public bool IsInitialize { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public Guid TemplateId { get; set; }

        public DateTime BroadcastSchedule { get; set; }
        public List<CampaignMessageButtonRequest>? Buttons { get; set; }
    }
}

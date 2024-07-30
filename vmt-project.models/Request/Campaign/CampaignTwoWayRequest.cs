using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.models.Request.Campaign
{
    public class CampaignTwoWayRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public IList<string> Keyword { get; set; }

        public IList<MediaConfigRequest>? MediaConfigs { get; set; } = new List<MediaConfigRequest>();
    }
}

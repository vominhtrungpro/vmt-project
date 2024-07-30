using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.models.Request.Campaign
{
    public class CampaignRecipientRequest
    {
        public bool IsUnconfirmed { get; set; }

        public bool IsSubscribe { get; set; }

        public bool IsUnsubscribe { get; set; }

        public IList<string>? TagFilters { get; set; } = new List<string>();
    }
}

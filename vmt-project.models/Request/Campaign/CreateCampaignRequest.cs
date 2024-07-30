using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.common.Enums;

namespace vmt_project.models.Request.Campaign
{
    public class CreateCampaignRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsEnableTwoWay { get; set; }
        [Required]
        public bool IsBypassUnsubBlock { get; set; }

        [Required]
        public CampaignStatus Status { get; set; }

        public string? EmailNotify { get; set; }

        public CampaignRecipientRequest RecipientRequest { get; set; }

        public IList<CampaignMessageRequest>? MessageRequests { get; set; }

        public IList<CampaignTwoWayRequest>? TwoWayRequests { get; set; }
        public bool? IsTurnOnFollowingMessage { get; set; }
    }
}

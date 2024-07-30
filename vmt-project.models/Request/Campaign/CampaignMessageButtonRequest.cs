using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.models.Request.Campaign
{
    public class CampaignMessageButtonRequest
    {
        public Guid? Id { get; set; }
        public Guid CampaignMessageId { get; set; }
        public string ButtonType { get; set; }


        public string? Text { get; set; }
        /// <summary>
        /// Gets or sets the phone number for phone-number type.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string? PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets the URL for url type.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string? Url { get; set; }
        /// <summary>
        /// Gets or sets the payload for quick_reply type.
        /// </summary>
        /// <value>
        /// The payload.
        /// </value>
        public string? Payload { get; set; }


        /// <summary>
        /// Gets or sets the URL suffix for url type.
        /// </summary>
        /// <value>
        /// The URL suffix.
        /// </value>
        public string? UrlSuffix { get; set; }
        public int? OrderNumber { get; set; }

        public string? ContentUrl { get; set; }
    }
}

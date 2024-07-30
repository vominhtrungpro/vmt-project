using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.common.Enums
{
    public enum CampaignStatus
    {
        Draft = 0,
        Pending = 1,
        Inprogress = 2,
        Completed = 3,
        Deleted = -1,
        Canceled = -2,
        None = 999
    }
}

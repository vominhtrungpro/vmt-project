using Nest;
using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.models.Request.Generic
{
    public class SearchRequest : BaseSearchRequest
    {
        public IList<Filter> Filters { get; set; }

        public SortByInfo SortBy { get; set; }
    }
}

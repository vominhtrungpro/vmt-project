using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.common.Enums;

namespace vmt_project.models.Request.Generic
{
    public class Filter
    {
        public string FieldName { get; set; }

        public string Value { get; set; }

        public OperationEnum Operation { get; set; }
    }
}

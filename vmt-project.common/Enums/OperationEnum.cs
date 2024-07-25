using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.common.Enums
{
    public enum OperationEnum
    {
        [Description("is")]
        Is,
        [Description("is more than")]
        IsMoreThan,
        [Description("is less than")]
        IsLessThan,
        [Description("contains")]
        Contain,
        [Description("")]
        Between,
        [Description("does not contains")]
        NotContain,

        [Description("is not")]
        IsNot,
        [Description("is more or equal to")]
        MoreOrEqual,
        [Description("is less or equal to")]
        LessOrEqual,
        [Description("is empty")]
        Empty,
        [Description("is not empty")]
        NotEmpty,
        [Description("starts with")]
        StartsWith,
        [Description("does not start with")]
        NotStartWith,
        [Description("ends with")]
        EndsWith,
        [Description("does not end with")]
        NotEndWith,
        [Description("is later than")]
        Later,
        [Description("is earlier than")]
        Earlier,
        [Description("is exactly or later than")]
        ExactlyOrLater,
        [Description("is exactly or earlier than")]
        ExactlyOrEarlier,
    }
}

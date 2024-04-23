using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.common.Helpers
{
    public static class ClaimHelper
    {
        public static string GetCurrentUserId(IHttpContextAccessor context)
        {
            return context.HttpContext.User.Claims.First(x => x.Type == "UserId").Value;
        }
    }
}

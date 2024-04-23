using Microsoft.AspNetCore.Identity;
using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.models.Request.Authentication;
using vmt_project.models.Response.Authentication;

namespace vmt_project.services.Contracts
{
    public interface IAuthenticationService
    {
        Task<AppActionResultData<LoginResponse>> Login(LoginRequest request);
        Task<AppActionResultData<IdentityResult>> Register(RegisterRequest request);
    }
}

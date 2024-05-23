using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Infrastructure.Api.Controller;
using vmt_project.common.Helpers;
using vmt_project.models.Request.Role;
using vmt_project.services.Contracts;
using static NetCore.Infrastructure.Api.ApiResultHelper;

namespace vmt_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RoleController(IRoleService roleService,IHttpContextAccessor httpContextAccessor)
        {
            _roleService = roleService;  
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                var result = await _roleService.ListRole();
                if (result.IsSuccess)
                {
                    return Success(result.Data,result.Detail);
                }
                return new OkObjectResult(BuildErrorApiResult(result.Detail));
            }
            catch (Exception ex)
            {
                return Success(ex.StackTrace);
            }
            finally
            {
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }

                var result = await _roleService.CreateRole(request);
                if (result.IsSuccess)
                {
                    return Success(result.Detail);
                }
                return new OkObjectResult(BuildErrorApiResult(result.Detail));
            }
            catch (Exception ex)
            {
                return Success(ex.StackTrace);
            }
            finally
            {
            }
        }
        [HttpPost]
        [Route("set-role")]
        public async Task<IActionResult> SetRole([FromBody] SetRoleRequest request)
        {
            try
            {
                var currentRole = ClaimHelper.GetCurrentRole(_httpContextAccessor);
                if (currentRole != "Super Admin")
                {
                    return Unauthorized();
                }
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }

                var result = await _roleService.SetRole(request);
                if (result.IsSuccess)
                {
                    return Success(result.Detail);
                }
                return new OkObjectResult(BuildErrorApiResult(result.Detail));
            }
            catch (Exception ex)
            {
                return Success(ex.StackTrace);
            }
            finally
            {
            }
        }
    }
}

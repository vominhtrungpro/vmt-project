using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Infrastructure.Api.Controller;
using vmt_project.models.Request.User;
using vmt_project.models.Request.UserInfo;
using vmt_project.services.Contracts;
using vmt_project.services.Implementations;
using static NetCore.Infrastructure.Api.ApiResultHelper;

namespace vmt_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserInfoController : BaseController
    {
        private readonly IUserInfoService _userInfoService;
        public UserInfoController(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserInfoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }

                var result = await _userInfoService.Upsert(request);
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

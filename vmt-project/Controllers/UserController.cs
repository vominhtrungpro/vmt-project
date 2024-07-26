using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Infrastructure.Api.Controller;
using Newtonsoft.Json;
using vmt_project.common.Helpers;
using vmt_project.models.Request.User;
using vmt_project.services.Contracts;
using static NetCore.Infrastructure.Api.ApiResultHelper;

namespace vmt_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserProfileById(string id)
        {
            try
            {
                var result = await _userService.GetUserProfileById(id);
                if (result.IsSuccess)
                {
                    return Success(result.Data, result.Detail);
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
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }
                request.UserId = ClaimHelper.GetCurrentUserId(_httpContextAccessor);
                var result = await _userService.ChangePasswordUserAsync(request);
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
        [AllowAnonymous]
        [Route("forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }

                var result = await _userService.ForgetPassword(request);
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
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }

                var result = await _userService.ResetPassword(request);
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

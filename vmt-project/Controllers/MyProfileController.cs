using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Infrastructure.Api.Controller;
using vmt_project.models.Request.MyProfile;
using vmt_project.services.Contracts;
using vmt_project.services.Implementations;

namespace vmt_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyProfileController : BaseController
    {
        private readonly IMyProfileService _myProfileService;
        public MyProfileController(IMyProfileService myProfileService)
        {
            _myProfileService = myProfileService; 
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List()
        {
            try
            {
                var result = await _myProfileService.List();
                return Success(result.Data, result.Detail);
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
        public async Task<IActionResult> Create([FromBody] CreateMyProfileRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }
                var result = await _myProfileService.Create(request);
                return Success(null, result.Detail);
            }
            catch (Exception ex)
            {
                return Success(ex.StackTrace);
            }
            finally
            {
            }
        }
        [HttpPut]
        [AllowAnonymous]
        public async Task<IActionResult> Update([FromBody] UpdateMyProfileRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }
                var result = await _myProfileService.Update(request);
                return Success(null, result.Detail);
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

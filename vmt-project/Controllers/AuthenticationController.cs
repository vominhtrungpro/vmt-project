using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using NetCore.Infrastructure.Api.Controller;
using vmt_project.models.Request.Authentication;
using vmt_project.services.Contracts;
using static NetCore.Infrastructure.Api.ApiResultHelper;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Http.HttpResults;
using Nest;
using vmt_project.dal.Models.Entities;
using vmt_project.models.DTO.Character;
using vmt_project.dal.Contracts;
using System.Data.Entity;
using vmt_project.dal.Implementations;
using System.Reflection.Metadata;
using vmt_project.services.Elastic;
using Serilog;

namespace vmt_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICharacterService _characterService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticationService authenticationService, ICharacterService characterService, ILogger<AuthenticationController> logger)
        {
            _authenticationService = authenticationService;
            _characterService = characterService;
            _logger = logger;
        }
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }

                var result = await _authenticationService.Login(request);
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
        [Route("login-google")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithGoogle([FromBody] LoginWithGoogleRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }

                var result = await _authenticationService.LoginWithGooogle(request);
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
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }

                var result = await _authenticationService.Register(request);
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
        [Route("test")]
        [AllowAnonymous]
        public async Task<IActionResult> Test(string text)
        {
            try
            {
                return Success(text);
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

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
using vmt_project.services.OpenAI;

namespace vmt_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICharacterService _characterService;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IChatOpenAIService _chatOpenAIService;

        public AuthenticationController(IAuthenticationService authenticationService, ICharacterService characterService, ILogger<AuthenticationController> logger, IChatOpenAIService chatOpenAIService)
        {
            _authenticationService = authenticationService;
            _characterService = characterService;
            _logger = logger;
            _chatOpenAIService = chatOpenAIService;
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
        public async Task<IActionResult> Test(string? text,string? threadId)
        {
            try
            {
                //var result = await _chatOpenAIService.AssistantChat(text,threadId);
                DateTimeOffset dateTimeWithOffset = new DateTimeOffset(2024, 7, 30, 2, 27, 29, 231, TimeSpan.FromHours(7));

                // Chuyển đổi thành chuỗi theo định dạng mặc định
                string defaultFormat = dateTimeWithOffset.ToString();
                Console.WriteLine("Default Format: " + defaultFormat);

                // Chuyển đổi thành chuỗi theo định dạng chuẩn
                string standardFormat = dateTimeWithOffset.ToString("o"); // Định dạng "o" (round-trip)
                Console.WriteLine("Standard Format (o): " + standardFormat);

                // Chuyển đổi thành chuỗi theo định dạng tùy chỉnh
                string customFormat = dateTimeWithOffset.ToString("yyyy-MM-dd HH:mm:ss zzz");

                DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(customFormat);

                DateTime utcDateTime = dateTimeOffset.UtcDateTime;


                return Success(customFormat);
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

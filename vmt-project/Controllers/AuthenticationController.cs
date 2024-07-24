﻿using Azure;
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

namespace vmt_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICharacterRepository _repo;
        public AuthenticationController(IAuthenticationService authenticationService, ICharacterRepository repo)
        {
            _authenticationService = authenticationService;
            _repo = repo;
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
                var settings = new ConnectionSettings(new Uri("https://c1e26596e2114447af4c0f0224e4a5de.eastus2.azure.elastic-cloud.com/"))
    .DefaultIndex("character-index")
    .BasicAuthentication("elastic", "LmKZuwze4VkJotLKRTtqbo2l");

                var client = new ElasticClient(settings);

                var searchResponse = client.Search<Character>(s => s
    .Query(q => q
        .Match(m => m
            .Field(f => f.Name)
            .Query(text)
        )
    )
);



                return Success(searchResponse.Documents);
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

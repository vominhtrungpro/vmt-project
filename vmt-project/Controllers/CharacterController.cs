﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Infrastructure.Api.Controller;
using Newtonsoft.Json;
using vmt_project.common.Helpers;
using vmt_project.models.Request.Character;
using vmt_project.services.Contracts;
using static NetCore.Infrastructure.Api.ApiResultHelper;

namespace vmt_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : BaseController
    {
        private readonly ICharacterService _characterService;
        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            try
            {
                var result = await _characterService.List();
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
        [HttpGet]
        [Route("{Id}")]
        public async Task<IActionResult> Get(Guid Id)
        {
            try
            {
                var result = await _characterService.Get_Dapper(Id);
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
        public async Task<IActionResult> Create([FromBody] CreateCharacterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }
                if (request.IsUsingDapper)
                {
                    var result = await _characterService.Create_Dapper(request);
                    return Success(null, result.Detail);
                }
                else
                {
                    var result = await _characterService.Create(request);
                    return Success(null, result.Detail);
                }   
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
        [Route("{Id}")]
        public async Task<IActionResult> Update([FromBody] UpdateCharacterRequest request, Guid Id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }
                var result = await _characterService.Update_Dapper(request, Id);
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
        [HttpDelete]
        [Route("{Id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }
                var result = await _characterService.Delete_Dapper(Id);
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
        [HttpPost]
        [Route("call-back")]
        public async Task<IActionResult> CallBack([FromBody] CallBackRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }
                CreateCharacterRequest createCharacterRequest = new CreateCharacterRequest()
                {
                    Name = JsonConvert.SerializeObject(request)

                };
                var result = await _characterService.Create(createCharacterRequest);
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
        [HttpPost]
        [Route("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromBody] SearchCharacterRequest request)
        {
            var dateTime = DateTime.UtcNow;
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }
                var result = await _characterService.Search(request);

                if (result.IsSuccess)
                {
                    return Success(result.Data, result.Detail);
                }
                return BadRequest(BuildErrorApiResult(result.Detail));
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

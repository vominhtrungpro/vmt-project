using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Infrastructure.Api.Controller;
using Newtonsoft.Json;
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
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCharacterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }
                var result = await _characterService.Create(request);
                if (result.IsSuccess)
                {
                    return Success(null, result.Detail);
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

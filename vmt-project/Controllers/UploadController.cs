using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using NetCore.Infrastructure.Api.Controller;
using NetCore.Infrastructure.Common.Models;
using Newtonsoft.Json;
using vmt_project.common.Helpers;
using vmt_project.models.Request.Upload;
using vmt_project.models.Request.User;
using vmt_project.services.Contracts;
using vmt_project.services.Implementations;
using static NetCore.Infrastructure.Api.ApiResultHelper;

namespace vmt_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : BaseController
    {
        private readonly IUploadService _uploadService;
        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }
        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload([FromForm] IEnumerable<IFormFile> files)
        {
            try
            {
                var results = new AppActionResultData<List<string>>();
                var urls = new List<string>();  
                if (files == null)
                {
                    return BadRequest(BuildErrorApiResult(""));
                }
                else
                {
                    foreach (var file in files)
                    {
                        var upload = await _uploadService.UploadImage(file);
                        if (upload.IsSuccess)
                        {
                            urls.Add(upload.Data);
                        }
                        else
                        {
                            return BadRequest(BuildErrorApiResult(upload.Detail));
                        }
                    }
                }
                return Success(urls);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        [Route("remove")]
        public async Task<IActionResult> Remove([FromBody] RemoveBlobRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return ClientError(ModelState);
                }
                else
                {
                    var result = await _uploadService.RemoveBlobs(request.Urls);
                    if (result.IsSuccess)
                    {
                        return Success(result.Detail);
                    }
                    return BadRequest(BuildErrorApiResult(result.Detail));
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

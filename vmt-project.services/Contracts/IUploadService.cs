using Microsoft.AspNetCore.Http;
using NetCore.Infrastructure.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vmt_project.services.Contracts
{
    public interface IUploadService
    {
        Task<AppActionResultData<string>> UploadImage(IFormFile file);
        Task<AppActionResult> RemoveBlobs(List<string> urls);
    }
}

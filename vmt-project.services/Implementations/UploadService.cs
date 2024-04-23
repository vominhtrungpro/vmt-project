using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NetCore.Infrastructure.Common.Models;
using NetTopologySuite.Index.HPRtree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vmt_project.common.Helpers;
using vmt_project.services.Contracts;

namespace vmt_project.services.Implementations
{
    public class UploadService : IUploadService
    {
        private readonly string _connectionString;
        private readonly string _imageContainerName;
        private readonly string _blobUrl;


        public UploadService(IConfiguration configuration)
        {
            _connectionString = Environment.GetEnvironmentVariable("StorageConnectionString");
            _imageContainerName = Environment.GetEnvironmentVariable("StorageImageContainerName");
            _blobUrl = Environment.GetEnvironmentVariable("StorageBlobUrl");
        }
        public async Task<AppActionResult> RemoveBlobs(List<string> urls)
        {
            var result = new AppActionResult();
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            foreach (var url in urls)
            {
                Uri uri = new Uri(url);
                string containerName = uri.Segments[1];
                string blobName = uri.Segments[2];
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);
                await blobClient.DeleteIfExistsAsync();
            }
            return result.BuildResult("Success!");
        }
        public async Task<AppActionResultData<string>> UploadImage(IFormFile file)
        {
            var result = new AppActionResultData<string>();
            string fileName = DateTime.UtcNow.ToString("yyMMdd_HHmmssfff_") + file.FileName;
            var type = FileHelper.GetTypeFromUrl(fileName);
            if (type is not null && type != "image")
            {
                return result.BuildError("File is not image!");
            }
            var fileContent = new MemoryStream();
            file.CopyTo(fileContent);
            var container = GetContainerImageBlob();
            if (!await container.ExistsAsync())
            {
                container = await InitContainerImageBlob();
            }
            var bobclient = container.GetBlobClient(fileName);

            string extension = fileName.Split('.').Last();
            var blobHttpHeader = new BlobHttpHeaders { ContentType = type + "/" + extension };
            if (!bobclient.Exists())
            {
                fileContent.Position = 0;
                await bobclient.UploadAsync(fileContent, new BlobUploadOptions { HttpHeaders = blobHttpHeader });
                return result.BuildResult(GetImageUrl(fileName));
            }
            else
            {
                fileContent.Position = 0;
                await bobclient.UploadAsync(fileContent, overwrite: true);
                return result.BuildResult(GetImageUrl(fileName));
            }
        }
        private string GetImageUrl(string fileName)
        {
            return $"{_blobUrl}/{_imageContainerName}/{fileName}";
        }
        private BlobContainerClient GetContainerImageBlob()
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            return blobServiceClient.GetBlobContainerClient(_imageContainerName);
        }
        private async Task<BlobContainerClient> InitContainerImageBlob()
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            await blobServiceClient.CreateBlobContainerAsync(_imageContainerName);
            return blobServiceClient.GetBlobContainerClient(_imageContainerName);
        }
    }
}

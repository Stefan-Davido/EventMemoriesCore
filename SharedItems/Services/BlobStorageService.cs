using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedItems.Services
{
    public interface IBlobStorageService
    {
        Task<string> UploadAsync(IFormFile file);
    }

    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobStorageService(IConfiguration config)
        {
            var connectionString = config["AzureBlobStorage:ConnectionString"];
            var containerName = config["AzureBlobStorage:ContainerName"];
            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.None); // keep private, serve via SAS
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var blobName = $"{Guid.NewGuid()}{extension}";
            var blobClient = _containerClient.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobHttpHeaders
            {
                ContentType = file.ContentType
            });

            return blobClient.Uri.ToString();
        }
    }
}

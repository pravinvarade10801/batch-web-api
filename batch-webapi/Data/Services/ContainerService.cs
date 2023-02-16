using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace batch_webapi.Data.Services
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public ContainerService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient= blobServiceClient;  
        }

        public async Task AddFile(string fileName,string filePath, string containerName, string contentType)
        { 
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            await container.SetAccessPolicyAsync(PublicAccessType.Blob);
            var blob = container.GetBlobClient(fileName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
            }
        }

        public bool CheckIfContainerExist(string containerName)
        {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            bool isExist = container.Exists();
            return isExist;
        }

        public async Task CreateContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        
    }
}

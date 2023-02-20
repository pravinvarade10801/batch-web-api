using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace batch_webapi.Data.Services
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _config;
        public ContainerService(BlobServiceClient blobServiceClient, IConfiguration config)
        {
            _blobServiceClient= blobServiceClient; 
            _config= config;
        }

        public async Task AddFile(string fileName,string filePath, string containerName, string contentType)
        { 
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            await container.SetAccessPolicyAsync(PublicAccessType.Blob);
            var blob = container.GetBlobClient(fileName);
            await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            

            var BlobContainer = _config.GetValue<string>("BlobContainer");
            await DownloadToStream(fileName,filePath,BlobContainer);


            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });
            }
        }
        private async Task DownloadToStream(string fileName, string filePath, string containerName)
        {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
            await container.SetAccessPolicyAsync(PublicAccessType.Blob);
            var blob = container.GetBlobClient(fileName);
            if (blob.Exists())
            {
                FileStream fileStream = File.OpenWrite(filePath);
                await blob.DownloadToAsync(fileStream);
                fileStream.Close();
            }
        }
        public bool GetFile(string fileName, string containerName)
        {
            var container = _blobServiceClient.GetBlobContainerClient(containerName);
             container.SetAccessPolicyAsync(PublicAccessType.Blob);
            var blob = container.GetBlobClient(fileName);
            if (blob.Exists())
            {
                return true;
            }
            return false;
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

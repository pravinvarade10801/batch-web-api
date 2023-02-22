
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using batch_webapi.Data.Models;
using batch_webapi.Data.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace batch_webapi.Data.Services
{
    /// <summary>
    /// Batch Service class which have post and get batch methods
    /// </summary>
    public class BatchService:IBatchService
    {
        private AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IContainerService _containerService;

        public BatchService(AppDbContext context, IConfiguration config, IContainerService containerService)
        {
            _context = context;
            _config = config;
            _containerService = containerService;
        }

        public Guid CreateBatch(BatchVM batch, Guid batchId)
        {
            var _batch = new Batch()
            {
                BatchId = batchId,
                BusinessUnit = batch.BusinessUnit,
                ExpiryDate = batch.ExpiryDate,
                BatchPublishedDate = DateTime.Now,
                Status = "InComplete"
            };
            //Instantiate ACL

            _batch.ACL = new ACL()
            {
                AclName = "Batch ACL"
            };

            //Add ReadUsers
            _batch.ACL.ReadUsers = new List<ReadUsers>();

            foreach (var readUsers in batch.ACL.ReadUsers)
            {
                _batch.ACL.ReadUsers.Add(new ReadUsers
                {
                    UserName = readUsers.ToString()
                });
            }


            //Add ReadGroups
            _batch.ACL.ReadGroups = new List<ReadGroups>();

            foreach (var readGroups in batch.ACL.ReadGroups)
            {
                _batch.ACL.ReadGroups.Add(new ReadGroups
                {
                    GroupName = readGroups.ToString()
                });
            }

            //Add Attributes
            _batch.Attributes = new Attributes();
            _batch.Attributes.Key = batch.Attributes.Key;
            _batch.Attributes.Value = batch.Attributes.Value;

            _context.Batch.Add(_batch);
            _context.SaveChanges();

            //Create Batch locally
            CreateBatchOnLocal(_batch.BatchId);

            //Create batch container in azure storage
            _containerService.CreateContainer(_batch.BatchId.ToString());

            return _batch.BatchId;
        }

        public BatchVMWithBatchDetails GetBatch(Guid batchId)
        {
            var batchUpload = _config.GetValue<string>("BatchUpload");
            List<FilesVM> filesVMs = new List<FilesVM>();
            FilesVM filesVM = new FilesVM();

            var files = _context.Files.Where(n => n.BatchId == batchId).ToList();
            if (files != null && files.Count > 0)
            {

                foreach (var file in files)
                {
                    filesVM = new FilesVM()
                    {
                        FileName = file.FileName,
                        FileSize = file.FileSize,
                        MimeType = file.MimeType,
                        Hash = file.Hash
                    };
                    filesVMs.Add(filesVM);
                }
            }


            var _batch = _context.Batch.Where(n => n.BatchId == batchId).Select(batch => new BatchVMWithBatchDetails()
            {
                BatchId = batchId,
                Status = batch.Status,
                BusinessUnit = batch.BusinessUnit,
                BatchPublisherDate = batch.BatchPublishedDate,
                ExpiryDate = batch.ExpiryDate,
                ACL = new ACLVM()
                {
                    ReadUsers = batch.ACL.ReadUsers.Where(p => p.AclId == batch.AclId)
                    .Select(n => n.UserName).ToList(),
                    ReadGroups = batch.ACL.ReadGroups.Where(p => p.AclId == batch.AclId)
                    .Select(n => n.GroupName).ToList()
                },
                Attributes = new AttributesVM()
                {
                    Key = batch.Attributes.Key,
                    Value = batch.Attributes.Value
                },
                Files = filesVMs

            }).FirstOrDefault();


            return _batch;
        }

        public void AddFileToBatch(Guid batchId, string filename, long X_Content_Size,
            string X_MIME_Type = null)
        {
            var batchUpload = _config.GetValue<string>("BatchUpload");
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), batchUpload, filename);

            X_MIME_Type = (X_MIME_Type == null) ? GetContentType(filename) : X_MIME_Type;

            //Add file to azure storage container
             _containerService.AddFile(filename, filePath, batchId.ToString(), X_MIME_Type);

            //Save file details in database
            var fileinfo = _context.Files.Where(f => f.BatchId == batchId && f.FileName == filename).FirstOrDefault();
            if (fileinfo == null)
            {
                Files file = new Files()
                {
                    BatchId = batchId,
                    FileName = filename,
                    FileSize = X_Content_Size,
                    MimeType = X_MIME_Type

                };
                _context.Files.Add(file);
                _context.SaveChanges();
            }

            //Update status of batch as Complete
            Batch batch = _context.Batch.Where(b => b.BatchId == batchId).FirstOrDefault();
            batch.Status = "Complete";
            _context.SaveChanges();

        }

        public bool CheckIfContainerExist(string containername)
        {
            return _containerService.CheckIfContainerExist(containername);
        }

        public bool CheckIfFileExist(string filename)
        {
            var blobContainer = _config.GetValue<string>("BlobContainer");

            //return File.Exists(Path.Combine(batchUpload, filename));
            return _containerService.GetFile(filename, blobContainer);


        }
        public bool CheckIfValidBatchId(string batchId)
        {
            Guid guidOutput;
            bool isValid = Guid.TryParse(batchId.ToString(), out guidOutput);
            return isValid;
        }
        private void CreateBatchOnLocal(Guid batchId)
        {
            var batchUpload = _config.GetValue<string>("BatchUpload");
            var pathToCreateBatch = Path.Combine(Directory.GetCurrentDirectory(), batchUpload);
            var filePath = Path.Combine(pathToCreateBatch, batchId.ToString());

            Directory.CreateDirectory(filePath);
        }
        private string GetContentType(string fileName)
        {

            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
            return contentType ?? "application/octet-stream";

        }
    }
}

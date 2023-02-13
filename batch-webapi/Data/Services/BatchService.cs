
using batch_webapi.Data.Models;
using batch_webapi.Data.ViewModels;
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
    public class BatchService
    {
        private AppDbContext _context;
        private readonly IConfiguration _config;
        
        public BatchService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public Guid CreateBatch(BatchVM batch, Guid batchId)
        {
            var batchUpload = _config.GetValue<string>("BatchUpload");
            //var batchUpload = "BatchUpload";
            var _batch = new Batch()
            {
                BatchId = batchId,
                BusinessUnit = batch.BusinessUnit,
                ExpiryDate = batch.ExpiryDate,
                BatchPublishedDate = DateTime.Now
            };
            //Instantiate ACL

            _batch.ACL = new ACL();

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

            //Create Batch
            
            var pathToCreateBatch = Path.Combine(Directory.GetCurrentDirectory(), batchUpload);
            var filePath = Path.Combine(pathToCreateBatch, _batch.BatchId.ToString());

            Directory.CreateDirectory(filePath);


            //Add json file in created batch
            var fileName = _batch.BatchId.ToString() + ".txt";
            var fileNamePath = Path.Combine(filePath, fileName);
            using (StreamWriter file = File.CreateText(fileNamePath))
            {
                string data = JsonConvert.SerializeObject(_batch, Newtonsoft.Json.Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, data);
            }

            return _batch.BatchId;
        }
        private string GetContentType(string fileName)
        {

            string contentType;
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out contentType);
            return contentType ?? "application/octet-stream";

        }
        public BatchVMWithBatchDetails GetBatch(Guid batchId)
        {

            var batchUpload = _config.GetValue<string>("BatchUpload");
            //var batchUpload = "BatchUpload";
            var batchPath = Path.Combine(Directory.GetCurrentDirectory(), batchUpload, batchId.ToString());

            List<FilesVM> filesVMs = new List<FilesVM>();
            FilesVM filesVM = new FilesVM();
            if (!Directory.Exists(batchPath))
            {
                return null;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(batchPath);
            FileInfo[] Files = directoryInfo.GetFiles();
            if (Files != null && Files.Length > 0)
            {
                
                foreach (FileInfo i in Files)
                {
                    filesVM = new FilesVM()
                    {
                        FileName = i.Name,
                        FileSize = i.Length,
                        MimeType = GetContentType(i.Name),
                        Hash = ""
                    };
                    filesVMs.Add(filesVM);
                }
            }

            var _batch = _context.Batch.Where(n => n.BatchId == batchId).Select(batch => new BatchVMWithBatchDetails()
            {
                BatchId = batchId,
                Status = "Incomplete",
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
    }
}

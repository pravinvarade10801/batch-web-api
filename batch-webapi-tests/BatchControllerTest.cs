using batch_webapi.Data.Services;
using batch_webapi.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using batch_webapi.Controllers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using batch_webapi.Data.ViewModels;
using batch_webapi.Exceptions;
using batch_webapi.Data.Models;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using FakeItEasy;
using Azure;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace batch_webapi_tests
{
    public class BatchControllerTest
    {
        private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: "BatchDbControllerTest")
          .Options;

        AppDbContext _context;              
        

        [OneTimeSetUp]
        public void Setup()
        {
            _context = new AppDbContext(dbContextOptions);
            _context.Database.EnsureCreated();

            SeedDatabase();          
            
        }

        [Test, Order(1)]
        public void HTTPPOST_CreateBatch_ReturnsCreatedAtActionResult_Test()
        {
            IConfiguration _config = A.Fake<IConfiguration>();
            ILogger<BatchController> _logger = A.Fake<ILogger<BatchController>>();
            BatchService _batchService = new BatchService(_context, _config);
            BatchController _batchController = new BatchController(_batchService, _logger);

            IActionResult actionResult = _batchController.CreateBatch(AssignBatch());

            Assert.That(actionResult, Is.TypeOf<CreatedAtActionResult>());

        }

        [Test, Order(2)]
        public void HTTPPOST_CreateBatch_ReturnsBadRequest_Test()
        {
            IConfiguration _config = A.Fake<IConfiguration>();
            ILogger<BatchController> _logger = A.Fake<ILogger<BatchController>>();
            BatchService _batchService = new BatchService(_context, _config);
            BatchController _batchController = new BatchController(_batchService, _logger);

            var newBatch = new BatchVM()
            {
                BusinessUnit = "",
                ACL = new ACLVM
                {
                    ReadUsers = new List<string>
                    {
                        "User 1","User 2"
                    },
                    ReadGroups = new List<string>
                    {
                        "G1","G2"
                    }

                },

                Attributes = new AttributesVM()
                {
                    Key = "Code",
                    Value = "ABC"
                },
                ExpiryDate = DateTime.Now.AddDays(1)

            };
            _batchController.ModelState.AddModelError("BusinessUnit", "mock error message");

            Assert.Throws<HttpStatusCodeException>(() => _batchController.CreateBatch(newBatch))
                .StatusCode.Equals(HttpStatusCode.BadRequest);

        }


        [Test, Order(3)]
        public void HTTPGET_GetBatchByBatchId_ReturnsBadRequest_HttpStatusCodeException_Test()
        {
            IConfiguration _config = A.Fake<IConfiguration>();
            ILogger<BatchController> _logger = A.Fake<ILogger<BatchController>>();
            BatchService _batchService = new BatchService(_context, _config);
            BatchController _batchController = new BatchController(_batchService, _logger);

            var batchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528");
            
            Assert.Throws<HttpStatusCodeException>(() => _batchController.GetBatchByBatchId(batchId)).StatusCode.Equals(HttpStatusCode.NotFound);
        }
        [Test, Order(4)]
        public void HTTPGET_GetBatchByBatchId_ReturnsNotFound_HttpStatusCodeException_Test()
        {
            IConfiguration _config = A.Fake<IConfiguration>();
            ILogger<BatchController> _logger = A.Fake<ILogger<BatchController>>();
            BatchService _batchService = new BatchService(_context, _config);
            BatchController _batchController = new BatchController(_batchService, _logger);

            var batchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528");

            Assert.Throws<HttpStatusCodeException>(() => _batchController.GetBatchByBatchId(batchId)).StatusCode.Equals(HttpStatusCode.BadRequest);
        }

        [Test, Order(5)]
        public void HTTPGET_GetBatchByBatchId_ReturnsOk_Test()
        {
            IConfiguration _config = A.Fake<IConfiguration>();
            ILogger<BatchController> _logger = A.Fake<ILogger<BatchController>>();
            BatchService _batchService = new BatchService(_context, _config);
            BatchController _batchController = new BatchController(_batchService, _logger);

            Guid batchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528");

            IActionResult actionResult = _batchController.GetBatchByBatchId(batchId);

            Assert.That(actionResult, Is.TypeOf<OkObjectResult>());

            var batchDeatails = (actionResult as OkObjectResult).Value as BatchVMWithBatchDetails;
            Assert.That(batchDeatails.BusinessUnit, Is.EqualTo("UKHO"));
            Assert.That(batchDeatails.BatchId, Is.EqualTo(batchId));

        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            _context.Database.EnsureDeleted();
        }
        private BatchVM AssignBatch()
        {
            var newBatch = new BatchVM()
            {
                BusinessUnit = "UKHO",
                ACL = new ACLVM
                {
                    ReadUsers = new List<string>
                    {
                        "User 1","User 2"
                    },
                    ReadGroups = new List<string>
                    {
                        "G1","G2"
                    }

                },

                Attributes = new AttributesVM()
                {
                    Key = "Code",
                    Value = "ABC"
                },
                ExpiryDate = DateTime.Now.AddDays(1)

            };
            return newBatch;
        }
        private void SeedDatabase()
        {

            var Attributes = new Attributes()
            {
                Key = "key",
                Value = "value"
            };
            _context.Attributes.AddRange(Attributes);

            var acl = new ACL()
            {
                AclId = 1
            };
            _context.ACL.AddRange(acl);
            var readuser = new ReadUsers()
            {
                UserName = "user",
                AclId = 1
            };
            _context.ReadUsers.Add(readuser);

            var readgroup = new ReadGroups()
            {
                GroupName = "group",
                AclId = 1
            };
            _context.AddRange(readgroup);

            var batch = new Batch()
            {
                BatchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528"),
                BusinessUnit = "UKHO",
                ExpiryDate = DateTime.Now.AddDays(10),
                BatchPublishedDate = DateTime.Now.AddDays(-10),
                AttributesId = 1
            };
            _context.Batch.AddRange(batch);
            _context.SaveChanges();
        }

    }
}

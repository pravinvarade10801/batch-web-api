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
using Microsoft.AspNetCore.Identity;

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
            IContainerService _containerService = A.Fake<IContainerService>();
            BatchService _batchService = new BatchService(_context, _config, _containerService);
            BatchController _batchController = new BatchController(_batchService, _logger);

            IActionResult actionResult = _batchController.CreateBatch(AssignBatch());

            Assert.That(actionResult, Is.TypeOf<CreatedAtActionResult>());

        }

        [Test, Order(2)]
        public void HTTPPOST_CreateBatch_ReturnsBadRequest_Test()
        {
            IConfiguration _config = A.Fake<IConfiguration>();
            IContainerService _containerService = A.Fake<IContainerService>();
            ILogger<BatchController> _logger = A.Fake<ILogger<BatchController>>();
            BatchService _batchService = new BatchService(_context, _config,_containerService);
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
            IContainerService _containerService = A.Fake<IContainerService>();
            ILogger<BatchController> _logger = A.Fake<ILogger<BatchController>>();
            BatchService _batchService = new BatchService(_context, _config, _containerService);
            BatchController _batchController = new BatchController(_batchService, _logger);

            var batchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528");
            
            Assert.Throws<HttpStatusCodeException>(() => _batchController.GetBatchByBatchId(batchId)).StatusCode.Equals(HttpStatusCode.NotFound);
        }
        [Test, Order(4)]
        public void HTTPGET_GetBatchByBatchId_ReturnsNotFound_HttpStatusCodeException_Test()
        {
            IConfiguration _config = A.Fake<IConfiguration>();
            IContainerService _containerService = A.Fake<IContainerService>();
            ILogger<BatchController> _logger = A.Fake<ILogger<BatchController>>();
            BatchService _batchService = new BatchService(_context, _config, _containerService);
            BatchController _batchController = new BatchController(_batchService, _logger);

            var batchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528");

            Assert.Throws<HttpStatusCodeException>(() => _batchController.GetBatchByBatchId(batchId)).StatusCode.Equals(HttpStatusCode.BadRequest);
        }

        [Test, Order(5)]
        public void HTTPGET_GetBatchByBatchId_ReturnsOk_Test()
        {
            IConfiguration _config = A.Fake<IConfiguration>();
            IContainerService _containerService = A.Fake<IContainerService>();
            ILogger<BatchController> _logger = A.Fake<ILogger<BatchController>>();
            BatchService _batchService = new BatchService(_context, _config, _containerService);
            BatchController _batchController = new BatchController(_batchService, _logger);

            Guid batchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528");

            IActionResult actionResult = _batchController.GetBatchByBatchId(batchId);

            Assert.That(actionResult, Is.TypeOf<OkObjectResult>());

            var batchDeatails = (actionResult as OkObjectResult).Value as BatchVMWithBatchDetails;
            Assert.That(batchDeatails.BusinessUnit, Is.EqualTo("UKHO"));
            Assert.That(batchDeatails.BatchId, Is.EqualTo(batchId));

        }

        [Test, Order(6)]
        public void HTTPPOST_AddFileToBatch_ReturnsCreatedResult_Test()
        {
            IConfiguration _config = A.Fake<IConfiguration>();
            ILogger<BatchController> _logger = A.Fake<ILogger<BatchController>>();
            IContainerService _containerService = A.Fake<IContainerService>();
            BatchService _batchService = new BatchService(_context, _config, _containerService);
            BatchController _batchController = new BatchController(_batchService, _logger);

            var batchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528");
            
            Task<IActionResult> actionResult = _batchController.AddFileToBatch(batchId,"photo.jpg",233,null);

            Assert.That(actionResult, Is.TypeOf<Task<CreatedResult>>());

        }

        [Test, Order(7)]
        public void HTTPPOST_AddFileToBatch_ReturnsBadRequestContainerNotExist_Test()
        {
            IConfiguration _config = A.Fake<IConfiguration>();
            IContainerService _containerService = A.Fake<IContainerService>();
            ILogger<BatchController> _logger = A.Fake<ILogger<BatchController>>();
            BatchService _batchService = new BatchService(_context, _config, _containerService);
            BatchController _batchController = new BatchController(_batchService, _logger);


            var batchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89523");

            Assert.Throws<HttpStatusCodeException>(() => _batchController.AddFileToBatch(batchId, "photo.jpg", 2323, null))
                .StatusCode.Equals(HttpStatusCode.BadRequest);
                

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
                       

            var batch = new Batch()
            {
                BatchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528"),
                BusinessUnit = "UKHO",
                ExpiryDate = DateTime.Now.AddDays(10),
                BatchPublishedDate = DateTime.Now.AddDays(-10),
                AttributesId = 1,
                  ACL = new ACL
                  {
                      ReadUsers = new List<ReadUsers>
                    {
                        new ReadUsers { 
                       UserName= "user1",
                        },
                        new ReadUsers{ 
                        UserName= "user2",
                        }

                    },
                      ReadGroups = new List<ReadGroups>
                    {
                        new ReadGroups {
                       GroupName= "g1",
                        },
                        new ReadGroups{
                        GroupName= "g2",
                        }

                    },
                  },

                Attributes = new Attributes()
                {
                    Key = "Code",
                    Value = "ABC"
                }                

            };
            _context.Batch.AddRange(batch);
            _context.SaveChanges();
        }

    }
}

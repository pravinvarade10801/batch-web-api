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

namespace batch_webapi_tests
{
    public class BatchControllerTest
    {
        private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: "BatchDbControllerTest")
          .Options;

        AppDbContext context;
        BatchService batchService;
        BatchController batchController;


        [OneTimeSetUp]
        public void Setup()
        {
            context = new AppDbContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDatabase();

            batchService = new BatchService(context, null);
            batchController = new BatchController(batchService, new NullLogger<BatchController>());
        }

        [Test, Order(1)]
        public void HTTPPOST_CreateBatch_ReturnsCreated_Test()
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

            IActionResult actionResult = batchController.CreateBatch(newBatch);

            Assert.That(actionResult, Is.TypeOf<CreatedAtActionResult>());
        }

        [Test, Order(2)]
        public void HTTPPOST_CreateBatch_ReturnsBadRequest_Test()
        {
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

            Assert.Throws<HttpStatusCodeException>(() => batchController.CreateBatch(newBatch));

        }

        [Test, Order(3)]
        public void HTTPGET_GetBatchByBatchId_ReturnsOk_Test()
        {
            Guid batchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528");

            IActionResult actionResult = batchController.GetBatchByBatchId(batchId);

            Assert.That(actionResult, Is.TypeOf<OkObjectResult>());

            var batchDeatails = (actionResult as OkObjectResult).Value as BatchVMWithBatchDetails;
            Assert.That(batchDeatails.BusinessUnit, Is.EqualTo("UKHO"));
            
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
        }
        private void SeedDatabase()
        {

            var Attributes = new Attributes()
            {
                Key = "key",
                Value = "value"
            };
            context.Attributes.AddRange(Attributes);

            var acl = new ACL()
            {
                AclId=1
            };
            context.ACL.AddRange(acl);
            var readuser = new ReadUsers()
            {
                UserName = "user",
                AclId = 1
            };
            context.ReadUsers.Add(readuser);

            var readgroup = new ReadGroups()
            {
                GroupName = "group",
                AclId = 1
            };
            context.AddRange(readgroup);

            var batch = new Batch()
            {
                BatchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528"),
                BusinessUnit = "UKHO",
                ExpiryDate = DateTime.Now.AddDays(10),
                BatchPublishedDate = DateTime.Now.AddDays(-10),
                AttributesId = 1
            };
            context.Batch.AddRange(batch);
            context.SaveChanges();
        }

    }
}

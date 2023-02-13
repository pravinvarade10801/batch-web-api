using batch_webapi.Data;
using batch_webapi.Data.Models;
using batch_webapi.Data.Services;
using batch_webapi.Data.ViewModels;
using batch_webapi.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;

namespace batch_webapi_tests
{
    public class BatchServiceTest
    {
        private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "BatchDbTest")
            .Options;

        AppDbContext context;
        BatchService batchService;

        [OneTimeSetUp]
        public void Setup()
        {
            context = new AppDbContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDatabase();
            batchService = new BatchService(context, null);
        }
       

        [Test, Order(1)]
        public void CreateBatch_WithoutException_Test()
        {
            //var config = new ConfigurationBuilder().AddJsonFile("~/appsetting.test.json").Build();

            //var batchUpload = Convert.ToString(config["BatchUpload"]);

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
            var batchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528");
            var result = batchService.CreateBatch(newBatch,batchId);

            Guid guidResult;
            Assert.IsTrue(Guid.TryParse(result.ToString(), out guidResult));

        }
        [Test, Order(2)]
        public void GetBatch_WithResponse_Test()
        {
            var batchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89528");
            var result = batchService.GetBatch(batchId);

            Assert.That(result.BatchId, Is.EqualTo(batchId));
            Assert.That(result.BusinessUnit, Is.EqualTo("UKHO"));
        }


        [Test, Order(3)]
        public void GetBatch_WithoutResponse_Test()
        {
            var batchId = new Guid("61EE6631-C7C5-40B3-B8DF-6345A1C89527");
            var result = batchService.GetBatch(batchId);

            Assert.That(result, Is.Null);
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

            var batch = new Batch()
            {
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
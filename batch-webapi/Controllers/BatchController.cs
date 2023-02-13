using batch_webapi.Data.Services;
using batch_webapi.Data.ViewModels;
using batch_webapi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace batch_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        public BatchService _batchService;
        private readonly ILogger<BatchController> _logger;

        public BatchController(BatchService batchService, ILogger<BatchController> logger)
        {
            _batchService = batchService;
            _logger = logger;
        }

        [HttpPost("create-batch")]
        public IActionResult CreateBatch([FromBody] BatchVM batch)
        {
            
            if (!ModelState.IsValid)
            { 
                var errorList = ModelState.Values.SelectMany(m => m.Errors)
                                 .Select(e => e.ErrorMessage)
                                 .ToList();
                var json = JsonSerializer.Serialize(errorList);
                _logger.LogInformation(nameof(CreateBatch) + " - " + json);
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, json);

            }
            var batchId = _batchService.CreateBatch(batch,Guid.NewGuid()).ToString();

            return Created(nameof(CreateBatch), new { batchId = batchId });


        }
        [HttpGet("get-batch-by-batchid/{batchId}")]
        public IActionResult GetBatchByBatchId(Guid batchId)
        {
            Guid guidOutput;
            bool isValid = Guid.TryParse(batchId.ToString(), out guidOutput);
            if (!isValid)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Invalid Batch Id");
            }

            var _response = _batchService.GetBatch(batchId);

            if (_response != null)
            {
                return Ok(_response);
            }
            else
            {
                _logger.LogInformation("Batch details not found using provided batchId: {batchId}", batchId);
                
                throw new HttpStatusCodeException(HttpStatusCode.NotFound, "Wrong Batch Id, No data found!");
            }

        }

    }

}

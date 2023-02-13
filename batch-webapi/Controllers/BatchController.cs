using batch_webapi.Data.Services;
using batch_webapi.Data.ViewModels;
using batch_webapi.Exceptions;
using Microsoft.AspNetCore.Http;
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
    /// <summary>
    /// Batch Controller which include Get and Post batch endpoints
    /// </summary>
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
        /// <summary>
        /// CreateBatch api endpoint
        /// </summary>
        /// <param name="batch"></param>
        /// <returns>Json - batchId</returns>
        /// <exception cref="HttpStatusCodeException"></exception>
        [HttpPost("create-batch")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
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

            return CreatedAtAction(nameof(CreateBatch), new { batchId = batchId });

        }

        /// <summary>
        /// get-batch-by-batchid api endpoint
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>Batch details Json</returns>
        /// <exception cref="HttpStatusCodeException"></exception>
        [HttpGet("get-batch-by-batchid/{batchId}")]        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetBatchByBatchId(Guid batchId)
        {
            //throw new Exception("error from middleware");
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

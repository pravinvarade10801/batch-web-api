using batch_webapi.Data.Models;
using batch_webapi.Data.Services;
using batch_webapi.Data.ViewModels;
using batch_webapi.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace batch_webapi.Controllers
{
    /// <summary>
    /// Batch Controller which include Get and Post batch endpoints
    /// </summary>
    
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly IBatchService _batchService;
        private readonly ILogger<BatchController> _logger;        
               
        public BatchController(IBatchService batchService, ILogger<BatchController> logger)
        {
            _batchService = batchService;
            _logger = logger;           
        }

        /// <summary>
        /// Create a new batch to upload files info
        /// </summary>
        /// <param></param>
        /// <returns>Json - batchId</returns>
        /// <exception cref="HttpStatusCodeException"></exception>
        [HttpPost("batch")]
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
                StringBuilder sb = new StringBuilder();
                foreach (var error in errorList)
                {
                    sb.Append(error);
                }
               
                _logger.LogInformation(nameof(CreateBatch) + " - " + sb.ToString());
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, sb.ToString());

            }
            var batchId = _batchService.CreateBatch(batch,Guid.NewGuid()).ToString();

            return CreatedAtAction(nameof(CreateBatch), new { batchId = batchId });

        }

        /// <summary>
        /// Get details of the batch including links to all the files in the batch
        /// </summary>
        /// <remarks>This get will include full details of the batch, for example it's status, the file in the batch</remarks>
        /// <param name="batchId">A Batch ID</param>        
        /// <returns>Batch details Json</returns>
        /// <exception cref="HttpStatusCodeException"></exception>
        [HttpGet("batch/{batchId}")]        
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


        /// <summary>
        /// Add a file to the batch
        /// </summary>
        /// <remarks>Creates a file in the bacth. To upload the content of the file,one or more uploadBlockOfFile requests will need to be made followed by a 'putBlocksInFile' request to complete the file.</remarks>
        /// <param name="batchId">A Batch ID</param>
        /// <param name="filename">Filename for the new file. Must be unique in the batch but can be the same as another file in another batch). Filenames don't include a path.</param>
        ///<param name="X_MIME_Type">Optional. The MIME content type of the file. The default type is application/octet-stream</param>
        ///<param name="X_Content_Size">The final size of the file in bytes</param>
        /// <returns></returns>
        /// <exception cref="HttpStatusCodeException"></exception>
        [HttpPost("batch/{batchId}/{filename}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddFileToBatch(Guid batchId, string filename,
            [FromHeader(Name= "X_Content_Size")][Required] long X_Content_Size,
            [FromHeader] string X_MIME_Type = null)
        {

            Guid guidOutput;
            bool isValid = Guid.TryParse(batchId.ToString(), out guidOutput);
            if (!isValid)
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Invalid Batch Id");
            }

            if (!_batchService.CheckIfContainerExist(batchId.ToString()))
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "Batch Id not exists");
            }

            if (!_batchService.CheckIfFileExist(filename))
            {
                throw new HttpStatusCodeException(HttpStatusCode.BadRequest, "File not exists");
            }
            
            await _batchService.AddFileToBatch(batchId,filename,X_Content_Size,X_MIME_Type);

            return Created(nameof(AddFileToBatch),null);

        }
        
    }
               
}

using LoggerService.Services.CorrelationId;
using LoggerService.Models;
using LoggerService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoggerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoggerController : ControllerBase
    {
        private ILoggingService _loggingService;
        private readonly ICorrelationIdGenerator _correlationIdGenerator;

        public LoggerController(ILoggingService loggingService, ICorrelationIdGenerator correlationIdGenerator)
        {
            _loggingService = loggingService;
            _correlationIdGenerator = correlationIdGenerator; 
        }


        /// <summary>
        /// Post a log message
        /// </summary>
        /// <param name="message"></param>
        /// <returns>HTTP 200 or error message if logging failed</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Message
        ///     {
        ///        "timestampUtc": "06.06.2023 16:46:24",
        ///        "message": "A critical error occurred",
        ///        "logLevel": 3
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Returns nothing</response>
        /// <response code="400">If request does not contain all or wrong parameter assignments</response>
        [HttpPost]
        public async Task<ActionResult<string>> PostMessage([FromBody] LogMessage message)
        {           
            string reply = await _loggingService.Log(message, _correlationIdGenerator.Get());

            if (string.IsNullOrEmpty(reply))
            {
                return Ok();
            }
            else
            {
                return BadRequest(reply);
            }
        }

    }
}

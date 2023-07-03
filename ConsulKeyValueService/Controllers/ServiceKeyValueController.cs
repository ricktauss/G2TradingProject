using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Consul;
using ConsulKeyValueService.Services;
using Newtonsoft.Json.Linq;

namespace ConsulKeyValueService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceKeyValueController : ControllerBase
    {

        private readonly KeyValueService _keyValueService;

        public ServiceKeyValueController(KeyValueService keyValueService) {

            _keyValueService  = keyValueService;

        }


        [HttpGet("{key}")]
        public async Task<ActionResult> GetKeyValue(string key)
        {
            ServiceResult<string> serviceResult = await _keyValueService.GetKeyValue(key);

            if (serviceResult.Success)
                return Ok(serviceResult);
            return BadRequest(serviceResult.ErrorMessage);
        }


        [HttpPost("{key}")]

        public async Task<ActionResult> PostKeyValue(string key, [FromBody] string value)
        {

           ServiceResult<string> serviceResult = await _keyValueService.SetKeyValue(key, value);

            if (serviceResult.Success)
                return Ok("Key has beed added successfully to Consul");
            return BadRequest(serviceResult.ErrorMessage);

        }

    }
}

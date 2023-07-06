using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Consul;
using ConsulKeyValueService.Services;
using Newtonsoft.Json.Linq;
using ConsulKeyValueService.Model;

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


        [HttpGet]
        public async Task<ActionResult> GetKeyValue([FromHeader(Name = "key")] string key)
        {
            ServiceResult<string> serviceResult = await _keyValueService.GetKeyValue(key);

            if (serviceResult.Success)
                return Ok(serviceResult.Data);
            return BadRequest(serviceResult.ErrorMessage);
        }


        [HttpPost]

        public async Task<ActionResult> PostKeyValue(SecretKeyValueModel secretKeyValueModel)
        {

           ServiceResult<string> serviceResult = await _keyValueService.SetKeyValue(secretKeyValueModel.key, secretKeyValueModel.value);

            if (serviceResult.Success)
                return Ok("Key has beed added successfully to Consul");
            return BadRequest(serviceResult.ErrorMessage);

        }

    }
}

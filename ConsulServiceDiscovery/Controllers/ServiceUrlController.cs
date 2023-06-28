using ConsulServiceDiscovery.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConsulServiceDiscovery.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServiceUrlController : ControllerBase
    {
        private readonly ILogger<ServiceUrlController> _logger;
        private readonly IUrlProvider _urlProvider;

        public ServiceUrlController(ILogger<ServiceUrlController> logger, IUrlProvider urlProvider)
        {
            _logger = logger;
            _urlProvider = urlProvider;
        }

        [HttpGet("{serviceTag}")]
        public ActionResult GetServiceUrl(string serviceTag)
        {
            var url = _urlProvider.GetUrlByTag(serviceTag);

            if (url == null)
            {
                return NotFound($"Service for service tag: \"{serviceTag}\" not found in service registry");
            }

            return Ok(url);
        }
    }
}
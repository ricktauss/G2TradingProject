using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SupplierService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet("")]
        [HttpHead("")]
        public IActionResult Ping()
        {
            return Ok();
        }

    }
}

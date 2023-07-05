using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplierService.Models;
using SupplierService.Service;

namespace SupplierService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private readonly ProductManagementService _productManagementservice;

        public ValuesController(ProductManagementService productManagementService) {

            _productManagementservice = productManagementService;

        }


        [HttpPost("{key}")]

        public async Task<ActionResult<string>> PostProduct(Product product, string key)
        {

          return await _productManagementservice.PostNewProduct(product, key);
          
        }


    }
}

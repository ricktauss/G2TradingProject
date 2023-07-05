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


        [HttpPost]

        public async Task<ActionResult<string>> PostProduct(Product product)
        {

          return await _productManagementservice.PostNewProduct(product, product.privateKey);
          
        }


    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SupplierService.Models;
using SupplierService.Service;
using System.Text.Json;

namespace SupplierService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {

        private readonly ProductManagementService _productManagementservice;

        public SupplierController(ProductManagementService productManagementService) {

            _productManagementservice = productManagementService;

        }


        [HttpPost]

     public async Task<IActionResult> PostProduct(Product product)
     {
            var response = await _productManagementservice.PostNewProduct(product, product.privateKey);

            if (response.IsSuccessStatusCode)
            {
                // If the request was successful, return the product.
                var content = await response.Content.ReadAsStringAsync();
                return Ok($"Request successfully: {content}");
            }
            else
            {
                // If the request failed, return a custom error message.
                var errorMessage = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"Request failed with message: {errorMessage}");
            }

        }


    }
}

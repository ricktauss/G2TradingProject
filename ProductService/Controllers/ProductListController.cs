using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using System.Collections;
using System.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductListController : ControllerBase
    {
        // GET: api/<ProductListController>
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7113/'");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await client.GetAsync("api/Products");


            if (response.IsSuccessStatusCode)
            {

                var content = await response.Content.ReadFromJsonAsync<List<Product>>();
                return content;

            }

            return null;

            /*
 Product product = new Product();

 product.Description = "Iphone";

 Product product2 = new Product();

 product2.Description = "Sony";

 List<Product> products = new List<Product>();
 products.Add(product);
 products.Add(product2);

     */

        }
    }


}

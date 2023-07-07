using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using System;
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
            var url = Environment.GetEnvironmentVariable("LocalDatastore_URL");
            client.BaseAddress = new Uri(url);
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


        }
    }


}

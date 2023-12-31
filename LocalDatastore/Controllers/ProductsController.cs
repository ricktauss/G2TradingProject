﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LocalDatastore.Models;
using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Http;
using System.Security.Policy;

namespace LocalDatastore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductContext _context;
        private readonly HttpClient _httpclient;


        public ProductsController(ProductContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpclient = httpClientFactory.CreateClient();
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetTodoItems()
        {
          if (_context.TodoItems == null)
          {
              return NotFound();
          }
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
          if (_context.TodoItems == null)
          {
              return NotFound();
          }
            var product = await _context.TodoItems.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<string>> PostProduct(Product product, [FromHeader(Name = "secret")] string secret)
        {
          if (_context.TodoItems == null)
          {
              return Problem("Entity set 'ProductContext.TodoItems'  is null.");
            }

            var url = Environment.GetEnvironmentVariable("ConsulKeyValueService_URL");

            var request = new HttpRequestMessage(HttpMethod.Get, url + "/api/ServiceKeyValue/");
            request.Headers.Add("key", "CreateProductKey");
            var response = await _httpclient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {

                if (await response.Content.ReadAsStringAsync() == secret) //check if secret is ok
                {
                    //Save product in DB
                    _context.TodoItems.Add(product);
                    await _context.SaveChangesAsync();

                    //Webhook: ProductServiceFTP has subscribed - Product will be transmitted to the FTP Server
                    var json = JsonConvert.SerializeObject(product);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Send the post request to the webhook URL - fire and forget :)
                    url = Environment.GetEnvironmentVariable("ProductServiceFTP_URL");
                    response = await _httpclient.PostAsync(url + "/api/ProductsfFTP", content);

                    CreatedAtAction("GetProduct", new { id = product.Id }, product);

                    return Ok(product.description + " has been successfully added to database!");

                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden, "User not authorized!");
                }


            }
            else
            {
                return BadRequest(response);
            }

          
           
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }
            var product = await _context.TodoItems.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

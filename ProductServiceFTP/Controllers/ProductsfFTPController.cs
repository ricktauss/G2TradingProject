using Microsoft.AspNetCore.Mvc;
using ProductServiceFTP.Models;
using System.Threading.Tasks;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProductServiceFTP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsfFTPController : ControllerBase
    {
        // GET: api/<ProductsfFTPController>
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetProducts()
        {

            string text = System.IO.File.ReadAllText(@"../ProductServiceFTP/Products.csv");
            string[] products = text.Split(new char[] {';'});

            System.Diagnostics.Debug.WriteLine(text);


            return Ok(products);
        }



        //Integrated Webhook Szenario

        [HttpPost]
        public async Task<ActionResult<Product>> PostWebhook(Product product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null");
            }

            string content = ";" + product.description;

            await System.IO.File.AppendAllTextAsync(@"../ProductServiceFTP/Products.csv",content);


            return Ok(product);

        }

    }
}

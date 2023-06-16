namespace OData;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using OData.Properties.Models;

public class ProductODataController : ODataController
{
    private List<Product> products = new List<Product>()
    {
        new Product(1, "Product 1", 10.99m),
        new Product(2, "Product 2", 19.99m),
        new Product(3, "Product 3", 5.99m)
    };

    [EnableQuery]
    public IEnumerable<Product> Get()
    {
        return products;
    }

    [EnableQuery]
    public IActionResult Get(int key)
    {
        var product = products.FirstOrDefault(p => p.Id == key);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    public IActionResult Post([FromBody] Product product)
    {
        products.Add(product);
        return Created(product);
    }

    public IActionResult Put(int key, [FromBody] Product product)
    {
        var existingProduct = products.FirstOrDefault(p => p.Id == key);
        if (existingProduct == null)
            return NotFound();

        existingProduct.Name = product.Name;
        existingProduct.Price = product.Price;

        return Updated(existingProduct);
    }

    public IActionResult Delete(int key)
    {
        var product = products.FirstOrDefault(p => p.Id == key);
        if (product == null)
            return NotFound();

        products.Remove(product);

        return NoContent();
    }
}
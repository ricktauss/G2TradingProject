namespace SupplierService.Service;
using Newtonsoft.Json;
using SupplierService.Models;


using System.Text;

public class ProductManagementService
{

    private readonly HttpClient _httpclient;
    private readonly IConfiguration _configuration;


    public ProductManagementService(IConfiguration configuration, IHttpClientFactory clientFactory) {

        _configuration = configuration;
        _httpclient = clientFactory.CreateClient();

    }


    public async Task<string> PostNewProduct(Product product, string key)
    {

        var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:7081/api/ServiceKeyValue/");
        request.Headers.Add("key", key);


        var response = await _httpclient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            return await response.Content.ReadAsStringAsync();
        }


/*
        var json = JsonConvert.SerializeObject(product);
        request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7050/api/ProductsfFTP");
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");


        response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }
        else
        {
            return null;
        }
*/

    }


}



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


    public async Task<HttpResponseMessage> PostNewProduct(Product product, string key)
    {
        var url = Environment.GetEnvironmentVariable("ConsulKeyValueService_URL");
        var request = new HttpRequestMessage(HttpMethod.Get, url+"/api/ServiceKeyValue/");
        request.Headers.Add("key", key);

      
        var response = await _httpclient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            url = Environment.GetEnvironmentVariable("LocalDatastore_URL");
            request = new HttpRequestMessage(HttpMethod.Post, url + "/api/Products/");
            request.Headers.Add("secret", await response.Content.ReadAsStringAsync());
            var productJson = JsonConvert.SerializeObject(product);
            request.Content = new StringContent(productJson, Encoding.UTF8, "application/json");
            response = await _httpclient.SendAsync(request);

            return response;

        }
        else 
        { 
            return response;
        }


    }


}



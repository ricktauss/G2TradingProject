using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using OData.Properties.Models;
using System.Net.Http.Json;

namespace OData
{
    public class ODataClient
    {
        private HttpClient httpClient;
        private string baseUri = "http://localhost:5108";

        public ODataClient()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(baseUri);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var response = await httpClient.GetAsync("Products");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var response = await httpClient.GetAsync($"Products({id})");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Product>();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            var response = await httpClient.PostAsJsonAsync("Products", product);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Product>();
        }

        public async Task<Product> UpdateProductAsync(int id, Product product)
        {
            var response = await httpClient.PutAsJsonAsync($"Products({id})", product);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Product>();
        }

        public async Task DeleteProductAsync(int id)
        {
            var response = await httpClient.DeleteAsync($"Products({id})");
            response.EnsureSuccessStatusCode();
        }
    }
}

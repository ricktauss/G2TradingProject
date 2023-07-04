namespace SupplierService.Service
{
    public class ProductManagementService
    {

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;


        public ProductManagementService(IConfiguration configuration, IHttpClientFactory clientFactory) {

            _configuration = configuration;
            _httpClientFactory = clientFactory;

        }


        public async Task<string> PostNewProduct(string uri, string description)
        {

            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Content = new StringContent(description);

            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode) 
            { 
                return await response.Content.ReadAsStringAsync();  
            }
            else
            {
                return null;
            }

        }


    }
}

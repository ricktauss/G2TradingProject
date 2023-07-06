using System.Net.Http.Headers;

namespace CreditcardService.Services
{
    public class ServiceDiscovery : IServiceDiscovery
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private string _serviceDiscoveryBaseAddress;

        public ServiceDiscovery(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = GetHttpClient();
        }

        public Uri GetUriByTag(string tag)
        {
            HttpResponseMessage response = _httpClient.GetAsync(_serviceDiscoveryBaseAddress + "/ServiceUrl/" + tag).Result;
            response.EnsureSuccessStatusCode();
            string uriString = response.Content.ReadAsStringAsync().Result;
            uriString = uriString.Trim('"');

            return new Uri(uriString);
        }

        private HttpClient GetHttpClient()
        {
            HttpClient client = _httpClientFactory.CreateClient("Default");
            _serviceDiscoveryBaseAddress = Environment.GetEnvironmentVariable("CREDITCARD_SERVICE_DISCOVERY_URL");
            client.BaseAddress = new Uri(_serviceDiscoveryBaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}

using Microsoft.AspNetCore.Mvc;

using System.Net.Http.Headers;
using Consul;
using Polly;
using CreditcardService.Services;
using CreditcardService.Models;
using CreditcardService.Services;

namespace BlackFriday.Controllers
{
    [Produces("application/json")]
    [Route("api/CashDesk")]
    public class CashDeskController : Controller
    {

        private readonly ILogger<CashDeskController> _logger;
        private readonly ICustomLogger _customLogger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceDiscovery _serviceDiscovery;


        public CashDeskController(ILogger<CashDeskController> logger,
            ICustomLogger customLogger,
            IHttpClientFactory httpClientFactory,
            IServiceDiscovery serviceDiscovery)
        {
            _logger = logger;
            _customLogger = customLogger;
            _httpClientFactory = httpClientFactory;
            _serviceDiscovery = serviceDiscovery;
        }

        [HttpGet]
        public IActionResult Get(string id)
        {
            return Content("OK");
        }

        [HttpPost]
        public IActionResult Post([FromBody] Basket basket)
        {
            _logger.LogInformation("TransactionInfo Creditcard: {0} Product:{1} Amount: {2}", new object[] { basket.CustomerCreditCardnumber, basket.Product, basket.AmountInEuro });

            //Mapping
            CreditcardTransaction creditCardTransaction = new CreditcardTransaction()
            {
                Amount = basket.AmountInEuro,
                CreditcardNumber = basket.CustomerCreditCardnumber,
                CreditcardType = basket.CreditcardType,
                ReceiverName = basket.Vendor
            };

            HttpClient client = GetHttpClient();
            _logger.LogInformation(" ######### ----------: " + client.BaseAddress);

            var retryPolicy = Polly.Policy
                .Handle<Exception>()
                .Retry(4, (exception, retryCount) =>
                {
                    Console.WriteLine($"Retry #{retryCount} due to {exception.Message}");
                    if (retryCount == 2)
                    {
                        //Get next URI
                        client.Dispose();
                        client = GetHttpClient();
                    }
                });

            retryPolicy.Execute(() =>
            {
                try
                {
                    HttpResponseMessage response = client.PostAsJsonAsync("/api/CreditcardTransactions", creditCardTransaction).Result;
                    response.EnsureSuccessStatusCode();

                }
                catch (Exception ex)
                {
                    _customLogger.LogError(ex.Message);
                    throw;
                }
            });

            return CreatedAtAction("Get", new { id = System.Guid.NewGuid() }, creditCardTransaction);
        }
      
        private HttpClient GetHttpClient()
        {
            HttpClient client = _httpClientFactory.CreateClient("Default");
            client.BaseAddress = _serviceDiscovery.GetUriByTag("CreditCard");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

    }
}
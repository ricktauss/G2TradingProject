using Microsoft.AspNetCore.Mvc;

using System.Net.Http.Headers;
using Consul;
using Polly;
using IEGEasyCreditcardService.Services;
using IEGEasyCreditcardService.Models;

namespace BlackFriday.Controllers
{
    [Produces("application/json")]
    [Route("api/CashDesk")]
    public class CashDeskController : Controller
    {

        private readonly ILogger<CashDeskController> _logger;
        private readonly ICustomLogger _customLogger;
        private static string creditcardServiceBaseAddress = "http://iegeasycreditcardservice.azurewebsites.net/";
        private LoadBalancerService _loadBalancerService;

        public CashDeskController(ILogger<CashDeskController> logger, ICustomLogger customLogger, LoadBalancerService loadBalancerService)
        {
            _logger = logger;
            _customLogger = customLogger;
            _loadBalancerService = loadBalancerService;
        }

        [HttpGet]
        public IActionResult Get(string id)
        {
            return Content("OK");
        }

        [HttpPost]
        public IActionResult Post([FromBody] Basket basket)
        {
            _logger.LogError("TransactionInfo Creditcard: {0} Product:{1} Amount: {2}", new object[] { basket.CustomerCreditCardnumber, basket.Product, basket.AmountInEuro });

            //Mapping
            CreditcardTransaction creditCardTransaction = new CreditcardTransaction()
            {
                Amount = basket.AmountInEuro,
                CreditcardNumber = basket.CustomerCreditCardnumber,
                CreditcardType = basket.CreditcardType,
                ReceiverName = basket.Vendor
            };

            HttpClient client = GetHttpClient();

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
                    HttpResponseMessage response = client.PostAsJsonAsync(creditcardServiceBaseAddress + "api/CreditcardTransactions", creditCardTransaction).Result;
                    response.EnsureSuccessStatusCode();

                } catch(Exception ex)
                {
                    _customLogger.LogError(ex.Message);
                    throw;
                } 
            });

            return CreatedAtAction("Get", new { id = System.Guid.NewGuid() }, creditCardTransaction);
        }

        /*
         * https://www.consul.io/
         * CreditcardTransactions.json 
         * {
            "service":{"name": "Payment/CreditcardTransaction",
            "Address": "https://localhost",
            "tags": ["CreditCard","Transaction"],"port": 56093,
            "check":	 {
                "id": "HealthCheckCreditCardService",
                "name": "HTTP API on port 5000",
                "http": "http://localhost:56093/api/Healthcheck",  
                "interval": "10s",
                "timeout": "1s"
  		            }
	            }
            }
        *c:\ consul agent -dev -enable-script-checks -config-dir=./config
        * http://localhost:8500/ui/dc1/services
         * 
         */

        private Uri GetCreditCardTransactionsURIFromConsul()
        {

            List<Uri> _serverUrls = new List<Uri>();
            var consuleClient = new ConsulClient(c => c.Address = new Uri("http://127.0.0.1:8500"));
            var services = consuleClient.Agent.Services().Result.Response;
            foreach (var service in services)
            {
                var isCreditCardApi = service.Value.Tags.Any(t => t == "CreditCard");
                if (isCreditCardApi)
                {
                    try
                    {
                        var serviceUri = new Uri($"{service.Value.Address}:{service.Value.Port}");
                        _serverUrls.Add(serviceUri);
                    }
                    catch (Exception)
                    {

                        ;
                    }

                }
            }

            return GetNextURI(_serverUrls); ;
        }

        /// <summary>
        /// Get next URI in round robin style
        /// </summary>
        /// <param name="uris"></param>
        /// <returns></returns>
        private Uri GetNextURI(List<Uri> uris)
        {
            int currentUrlIndex = _loadBalancerService.GetBalance(typeof(CashDeskController).Name);
            if (currentUrlIndex >= uris.Count)
            {
                currentUrlIndex = _loadBalancerService.SetBalance(typeof(CashDeskController).Name, 0);
            }
            Uri uri = uris[currentUrlIndex];

            return uri;
        }

        private HttpClient GetHttpClient()
        {
            HttpClient client = new HttpClient();
            creditcardServiceBaseAddress = GetCreditCardTransactionsURIFromConsul().AbsoluteUri;
            client.BaseAddress = new Uri(creditcardServiceBaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

    }
}
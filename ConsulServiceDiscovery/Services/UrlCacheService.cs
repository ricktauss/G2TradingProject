using Consul;
using System.Net.Http.Headers;

namespace ConsulServiceDiscovery.Services
{
    public class UrlCacheService : IHostedService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        private Uri _consulServerUrl;
        private Timer _timerFetchUrls;
        private Dictionary<string, List<Uri>> _urlCache;

        public UrlCacheService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _urlCache = new Dictionary<string, List<Uri>>();

            _httpClient = httpClientFactory.CreateClient("Default");
            initializeHttpClient(_httpClient, configuration);
        }

        private void initializeHttpClient(HttpClient _httpClient, IConfiguration configuration)
        {
            string consulServerUrlAsString = Environment.GetEnvironmentVariable("CONSUL_SERVER_URL");
            if (string.IsNullOrEmpty(consulServerUrlAsString))
            {
                throw new InvalidOperationException($"CONSUL_SERVER_URL environment variable is not defined: '{consulServerUrlAsString}'");
            }

            if (!Uri.TryCreate(consulServerUrlAsString, UriKind.Absolute, out _consulServerUrl))
            {
                throw new InvalidOperationException($"CONSUL_SERVER_URL environment variable is not a valid URL: '{consulServerUrlAsString}'");
            }

            _httpClient.BaseAddress = _consulServerUrl;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timerFetchUrls = new Timer(RefreshUrlsTimedAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            return Task.CompletedTask;
        }

        private async void RefreshUrlsTimedAsync(object state)
        {
            try
            {
                await _semaphore.WaitAsync();
                await RefreshUrlsAsync();

            } finally
            {
                _semaphore.Release();
            }  
        }


        private async Task RefreshUrlsAsync()
        {
            var newUrlCache =  await FetchUrlsAsync();

            if (newUrlCache != null)
            {
                _urlCache = newUrlCache;
            }
        }

        private async Task<Dictionary<string, List<Uri>>> FetchUrlsAsync()
        {
            Dictionary<string, List<Uri>> newUrlCache = _urlCache.Keys.ToDictionary(key => key, val => new List<Uri>());

            try
            {
                var consuleClient = new ConsulClient(c => c.Address = _consulServerUrl);
                var consulResult = await consuleClient.Agent.Services();
                var services = consulResult.Response;

                foreach (var service in services)
                {
                    foreach (string serviceTag in newUrlCache.Keys)
                    {
                        bool serviceEqualsTag = service.Value.Tags.Any(t => t.Equals(serviceTag));

                        if (serviceEqualsTag)
                        {
                            if (newUrlCache[serviceTag] == null)
                            {
                                newUrlCache[serviceTag] = new List<Uri>();
                            }

                            var serviceUri = new Uri($"{service.Value.Address}:{service.Value.Port}");

                            newUrlCache[serviceTag].Add(serviceUri);
                        }
                    }
                }

                return newUrlCache;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public async Task<List<Uri>> GetUrlsByTagAsync(string tag)
        {
            List<Uri> urls = null;
            bool lockActivated = false;

            try
            {
                if (!_urlCache.ContainsKey(tag))
                {
                    _urlCache.Add(tag, new List<Uri>());

                    await _semaphore.WaitAsync();
                    lockActivated = true;

                    await RefreshUrlsAsync();  
                }

                urls = _urlCache[tag];
            }
            finally
            {
                if(lockActivated)
                {
                    _semaphore.Release();
                }    
            }

            return urls;
        }

        public Dictionary<string, List<Uri>> GetAllUrls()
        {
            return _urlCache;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timerFetchUrls?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timerFetchUrls?.Dispose();
        }
    }
}

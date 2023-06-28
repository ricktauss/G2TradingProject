namespace ConsulServiceDiscovery.Services
{
    public class UrlProvider : IUrlProvider
    {
        private readonly UrlCacheService _urlCacheService;
        private readonly IUrlSelectionStrategy _urlSelectionStrategy;

        public UrlProvider(UrlCacheService urlCacheService, IUrlSelectionStrategy urlSelectionStrategy) 
        {
            _urlCacheService = urlCacheService;
            _urlSelectionStrategy = urlSelectionStrategy;
        }

        public Uri GetUrlByTag(string serviceTag)
        {
            var urls = _urlCacheService.GetUrlsByTagAsync(serviceTag).Result;
            return _urlSelectionStrategy.SelectUrl(urls);
        }
    }
}

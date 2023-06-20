namespace ConsulServiceDiscovery.Services
{
    public interface IUrlProvider
    {
        Uri GetUrlByTag(string serviceTag);
    }
}

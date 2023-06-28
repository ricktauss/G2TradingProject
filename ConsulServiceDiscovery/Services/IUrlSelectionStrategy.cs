namespace ConsulServiceDiscovery.Services
{
    public interface IUrlSelectionStrategy
    {
        Uri SelectUrl(List<Uri> urls);
    }
}

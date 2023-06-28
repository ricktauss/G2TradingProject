namespace ConsulServiceDiscovery.Services
{
    public class RoundRobinUrlSelectionStrategy : IUrlSelectionStrategy
    {
        private int _currentIndex = -1;
        private readonly object _lock = new object();

        public Uri SelectUrl(List<Uri> urls)
        {
            if (urls == null || urls.Count == 0)
            {
                return null;
            }

            lock (_lock)
            {
                _currentIndex = (_currentIndex + 1) % urls.Count;
                return urls[_currentIndex];
            }
        }
    }
}

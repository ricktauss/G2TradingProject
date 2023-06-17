using SharedModels.Models;
using System.Net.Http.Headers;

namespace IEGEasyCreditcardService.Services
{
    public class CustomLoggerService : ICustomLogger
    {
        private readonly ILogger<CustomLoggerService> _logger;
        private readonly HttpClient _httpClient;

        private readonly string _customLggerUri = "https://localhost:5011/";


        public CustomLoggerService(ILogger<CustomLoggerService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;

            _httpClient = httpClientFactory.CreateClient("Default");
            _httpClient.BaseAddress = new Uri(_customLggerUri);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        public void LogInformation(string message)
        {
            Send(message, 1);
        }

        public void LogWarning(string message)
        {
            Send(message, 2);
        }

        public void LogError(string message)
        {
            Send(message, 3);
        }


        private void Send(string message, int logLevel)
        {
            LogMessage logMessage = CreateNewLogMessage(message, logLevel);

            try
            {
                HttpResponseMessage response = _httpClient.PostAsJsonAsync(_customLggerUri + "api/Logger", logMessage).Result;

            }
            catch (Exception ex)
            {
                _logger.LogError("Log to central logging service failed: " + ex);
            }
        }

        private LogMessage CreateNewLogMessage(string message, int logLevel)
        {
            return new LogMessage
            {
                TimestampUtc = DateTime.UtcNow.ToString(),
                LogLevel = logLevel,
                Message = message
            };
        }
    }
}

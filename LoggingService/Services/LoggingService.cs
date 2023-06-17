using SharedModels.Models;

namespace LoggerService.Services
{
    public class LoggingService : ILoggingService
    {
        private ILogger<LoggingService> _logger;

        private readonly int _minLogLevel = 1;
        private readonly int _maxLogLevel = 3;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public Task<string> Log(LogMessage logMessage)
        {
            return Log(logMessage, null);
        }

        /// <summary>
        /// Log message and replies with boolean value 
        /// </summary>
        /// <param name="logMessage"></param>
        /// <returns>
        /// Tuple with boolean value indicating if logging was successfull and string with information if failed
        /// T1: false => logging failed  T2: true => logging successfull
        /// </returns>
        public Task<string> Log(LogMessage logMessage, string correlationId)
        {
            return Task.Run( () =>
            {
                try
                {
                    if (_logger == null)
                    {
                        return "No logger service available";
                    }
                    else if (string.IsNullOrEmpty(logMessage.Message))
                    {
                        return "Message is missing in request";
                    }

                    DateTime timestamp;
                    bool isValidDateTime = DateTime.TryParse(logMessage.TimestampUtc, out timestamp);

                    string logEntry = "";

                    if (isValidDateTime)
                        logEntry += $"{timestamp}";

                    if (!string.IsNullOrWhiteSpace(correlationId))
                        logEntry += $" - CorrID: {correlationId}";

                    logEntry += $" - {logMessage.Message}";

                    switch (logMessage.LogLevel)
                    {
                        case 1:
                            _logger.LogInformation(logEntry);
                            break;

                        case 2:
                            _logger.LogWarning(logEntry);
                            break;

                        case 3:
                            _logger.LogError(logEntry);
                            break;

                        default:
                            _logger.LogDebug(logEntry);
                            break;
                    }

                    return "";
                }
                catch (Exception ex)
                {
                    return "Logging failed - please try again later";
                }
            });
        }
        
    }
}

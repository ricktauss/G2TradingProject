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

        /// <summary>
        /// Log message and replies with boolean value 
        /// </summary>
        /// <param name="logMessage"></param>
        /// <returns>
        /// Tuple with boolean value indicating if logging was successfull and string with information if failed
        /// T1: false => logging failed  T2: true => logging successfull
        /// </returns>
        public Task<string> Log(LogMessage logMessage)
        {
            return Task.Run( () =>
            {
                try
                {
                    if (_logger == null)
                    {
                        return "No logger service available";
                    }
                    else if (string.IsNullOrEmpty(logMessage.TimestampUtc))
                    {
                        return "Timestamp is missing in request";
                    }
                    else if (string.IsNullOrEmpty(logMessage.Message))
                    {
                        return "Message is missing in request";
                    }
                    else if (logMessage.LogLevel < _minLogLevel || logMessage.LogLevel > _maxLogLevel)
                    {
                        return $"Log level must be between {_minLogLevel} and {_maxLogLevel}";
                    }

                    DateTime timestamp;
                    bool isValidDateTime = DateTime.TryParse(logMessage.TimestampUtc, out timestamp);

                    if (!isValidDateTime)
                    {
                        return "Timestamp format not valid";
                    }

                    string logEntry = $"{timestamp} - {logMessage.Message}";

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
                            return "Log level not valid";
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

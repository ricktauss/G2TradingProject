using SharedModels.Models;

namespace LoggerService.Services
{
    public interface ILoggingService
    {
        Task<string> Log(LogMessage logMessage);
    }
}

namespace IEGEasyCreditcardService.Services
{
    public interface ICustomLogger
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}

namespace LoggerService.Services.CorrelationId
{
    public interface ICorrelationIdGenerator
    {
        string Get();
        void Set(string correlationId);
    }
}

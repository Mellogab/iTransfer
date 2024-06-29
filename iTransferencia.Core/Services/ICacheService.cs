namespace iTransferencia.Core.Services
{
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow);
        bool TryGetValue<T>(string key, out T value);
    }
}

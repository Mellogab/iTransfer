namespace iTransferencia.Infrastructure.Configurations
{
    public class Retry
    {
        public int MaxRetryAttempts { get; set; }
        public int ExponentialBackoffBaseSeconds { get; set; }
    }
}

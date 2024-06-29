namespace iTransferencia.Infrastructure.Configurations
{
    public class CircuitBreaker
    {
        public int HandledEventsAllowedBeforeBreaking { get; set; }
        public int DurationOfBreakMinutes { get; set; }
    }
}

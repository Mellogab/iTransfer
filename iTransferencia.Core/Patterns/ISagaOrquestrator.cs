namespace iTransferencia.Core.Patterns
{
    public interface ISagaOrquestrator
    {
        public void AddStep(Func<Task> operation, Func<Task> compensation);
        public Task ExecuteAsync();
    }
}

using iTransferencia.Core.Patterns;

namespace iTransferencia.Infrastructure.Patterns.Sagas
{
    public class SagaOrquestrator : ISagaOrquestrator
    {
        private readonly List<Func<Task>> _operations = new();
        private readonly List<Func<Task>> _compensations = new();

        public void AddStep(Func<Task> operation, Func<Task> compensation)
        {
            _operations.Add(operation);
            _compensations.Insert(0, compensation);
        }

        public async Task ExecuteAsync()
        {
            foreach (var operation in _operations)
            {
                try
                {
                    await operation();
                }
                catch
                {
                    await CompensateAsync();
                    throw;
                }
            }
        }

        private async Task CompensateAsync()
        {
            foreach (var compensation in _compensations)
            {
                await compensation();
            }
        }
    }
}

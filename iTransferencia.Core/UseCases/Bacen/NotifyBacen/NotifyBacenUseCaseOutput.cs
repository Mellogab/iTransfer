namespace iTransferencia.Core.UseCases.Bacen.NotifyBacen
{
    public class NotifyBacenUseCaseOutput : UseCaseResponseMessage
    {
        public NotifyBacenUseCaseOutput(
            bool success,
            string message = null,
            string error = null
        ) : base(success, message, error)
        {
        }

        public NotifyBacenUseCaseOutput(
            bool success
        ) : base(success, null, null)
        {  
        }
    }
}
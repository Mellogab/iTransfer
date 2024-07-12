namespace iTransferencia.Core.UseCases.Transfers.ExecuteTransfer
{
    public class ExecuteTransferOutput : UseCaseResponseMessage
    {
        public Guid transaction_id { get; set; }
        public ExecuteTransferOutput(
            bool success,
            string message = null,
            string error = null
        ) : base(success, message, error)
        {
        }

        public ExecuteTransferOutput(
            bool success,
            Guid transaction_id,
            string message = null
        ) : base(success, message, null)
        {
            this.transaction_id = transaction_id;
        }
    }
}

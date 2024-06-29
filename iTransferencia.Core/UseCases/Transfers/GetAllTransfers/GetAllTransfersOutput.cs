namespace iTransferencia.Core.UseCases.Transfers.GetAllTransfers
{
    public class GetAllTransfersOutput : UseCaseResponseMessage
    {
        public List<Entities.Transfer> Transfers { get; set; }

        public GetAllTransfersOutput(
            bool success,
            string message = null,
            string error = null
        ) : base(success, message, error)
        {
        }

        public GetAllTransfersOutput(
            List<Entities.Transfer> transfers,
            bool success
        ) : base(success, null, null)
        {
            Transfers = transfers;
        }
    }
}

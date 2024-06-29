using iTransferencia.Core.UseCases.Transfers.GetAllTransfers;
using static iTransferencia.Core.IUseCaseRequest;

namespace iTransferencia.Core.UseCases.Transfers.ExecuteTransfer
{
    public class ExecuteTransferInput : IUseCaseRequest<ExecuteTransferOutput>
    {
        public string IdClient { get; set; }
        public string IdSourceAccount { get; set; }
        public string IdDestinationAccount { get; set; }
        public decimal Value { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}

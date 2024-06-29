using static iTransferencia.Core.IUseCaseRequest;

namespace iTransferencia.Core.UseCases.Transfers.GetAllTransfers
{
    public class GetAllTransfersInput : IUseCaseRequest<GetAllTransfersOutput>
    {
        public string Id { get; set; }
        public CancellationToken CancellationToken { get; set; }
    }
}

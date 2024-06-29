using iTransferencia.Core.Repository;

namespace iTransferencia.Core.UseCases.Transfers.GetAllTransfers
{
    public class GetAllTransfersUseCase : IGetAllTransfers
    {
        private ITransferRepository _ITransferRepository;
        
        public GetAllTransfersUseCase(
            ITransferRepository _iTransferRepository
        )
        {
            _ITransferRepository = _iTransferRepository;
        }

        public async Task<bool> Handle(GetAllTransfersInput message, IOutputPort<GetAllTransfersOutput> outputPort)
        {
            try
            {
                var payments = await _ITransferRepository.GetAllAsync(message.CancellationToken);
                outputPort.Handle(new GetAllTransfersOutput(payments.ToList(), true));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}

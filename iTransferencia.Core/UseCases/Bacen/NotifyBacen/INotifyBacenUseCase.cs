using iTransferencia.Core.UseCases.Transfers.GetAllTransfers;

namespace iTransferencia.Core.UseCases.Bacen.NotifyBacen
{
    public interface INotifyBacenUseCase : IUseCaseRequestHandler<NotifyBacenUseCaseInput, NotifyBacenUseCaseOutput>
    {
    }
}
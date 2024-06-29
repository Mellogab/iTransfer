using iTransferencia.Core.Enums;
using iTransferencia.Core.Patterns;
using iTransferencia.Core.Services;

namespace iTransferencia.Core.UseCases.Accounts.UpdateBalances
{
    public class UpdateBalancesUseCase : IUpdateBalancesUseCase
    {
        private IAccountService _SourceAccountService;
        private IAccountService _DestinationAccountService;
        private ISagaOrquestrator _SagaOrquestrator;

        public UpdateBalancesUseCase(
            IAccountService _sourceAccountService,
            IAccountService _destinationAccountService,
            ISagaOrquestrator _sagaOrquestrator
        )
        {
            this._SourceAccountService = _sourceAccountService;
            this._DestinationAccountService = _destinationAccountService;
            this._SagaOrquestrator = _sagaOrquestrator;
        }
        
        public async Task<bool> Handle(UpdateBalancesInput message, IOutputPort<UpdateBalancesOutput> outputPort)
        {
            var sourceAccount = await this._SourceAccountService.GetClientById(message.IdSourceAccount);

            if (sourceAccount is null)
            {
                outputPort.Handle(new UpdateBalancesOutput(false, "Conta origem não encontrada"));
                return false;
            }

            var updateSourceBalances = new Entities.Adapters.UpdateBalances(
                sourceAccount.saldo - message.Value, 
                message.IdSourceAccount, 
                message.IdDestinationAccount
            );
            
            var updateSourceBalancesRollback = new Entities.Adapters.UpdateBalances(
                sourceAccount.saldo, 
                message.IdSourceAccount, 
                message.IdDestinationAccount
            );

            this._SagaOrquestrator.AddStep(
                async () => await _SourceAccountService.UpdateBalances(updateSourceBalances),
                async () => await _SourceAccountService.UpdateBalances(updateSourceBalancesRollback)
            );

            var destinationAccount = await this._DestinationAccountService.GetClientById(message.IdDestinationAccount);

            if (destinationAccount is null)
            {
                outputPort.Handle(new UpdateBalancesOutput(false, "Conta destino não encontrada"));
                return false;
            }

            var updateDestinationBalances = new Entities.Adapters.UpdateBalances(
                destinationAccount.saldo + message.Value,
                message.IdDestinationAccount,
                message.IdSourceAccount
            );

            var updateDestinationBalancesRollback = new Entities.Adapters.UpdateBalances(
                destinationAccount.saldo,
                message.IdDestinationAccount,
                message.IdSourceAccount
            );

            this._SagaOrquestrator.AddStep(
                async () => await _DestinationAccountService.UpdateBalances(updateDestinationBalances),
                async () => await _DestinationAccountService.UpdateBalances(updateDestinationBalancesRollback)
            );

            try
            {
                await this._SagaOrquestrator.ExecuteAsync();
                outputPort.Handle(new UpdateBalancesOutput(TransferStatuses.SUCCESSFULLY_UPDATE_BALANCES, true));
                return true;
            }
            catch
            {
                outputPort.Handle(new UpdateBalancesOutput(TransferStatuses.FAILURE_UPDATE_BALANCES, false));
                return false;
            }
        }
    }
}
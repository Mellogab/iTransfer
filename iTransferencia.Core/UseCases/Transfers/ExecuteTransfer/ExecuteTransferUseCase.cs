using iTransferencia.Core.Entities;
using iTransferencia.Core.Enums;
using iTransferencia.Core.Extensions;
using iTransferencia.Core.Presenters;
using iTransferencia.Core.Repository;
using iTransferencia.Core.Services;
using iTransferencia.Core.UseCases.Accounts.UpdateBalances;
using iTransferencia.Core.UseCases.Bacen.NotifyBacen;
using Microsoft.Extensions.Logging;

namespace iTransferencia.Core.UseCases.Transfers.ExecuteTransfer
{
    public class ExecuteTransferUseCase : IExecuteTransferUseCase
    {
        private IIdempotenceRepository _IIdempotenceRepository { get; set; }
        private ITransferRepository _ITransferRepository { get; set; }
        private IClientService _IClientService { get; set; }
        private IAccountService _IAccountService { get; set; }
        private IUpdateBalancesUseCase _UpdateBalancesUseCase { get; set; }
        private INotifyBacenUseCase _INotifyBacenUseCase { get; set; }
        private readonly ILogger<ExecuteTransferUseCase> _Logger;
        private DefaultPresenter<UpdateBalancesOutput> _UpdateBalancesUseCasePresenter;
        private DefaultPresenter<NotifyBacenUseCaseOutput> _NotifyBacenUseCasePresenter;

        public ExecuteTransferUseCase(
            IIdempotenceRepository _iIdempotenceRepository,
            ITransferRepository _iTransferRepository,
            IClientService _iClientService,
            IAccountService _iAccountService,
            IUpdateBalancesUseCase _updateBalancesUseCase,
            INotifyBacenUseCase _iNotifyBacenUseCase,
            ILogger<ExecuteTransferUseCase> _logger,
            DefaultPresenter<UpdateBalancesOutput> _updateBalancesUseCasePresenter,
            DefaultPresenter<NotifyBacenUseCaseOutput> _notifyBacenUseCasePresenter
        )
        {
            this._IIdempotenceRepository = _iIdempotenceRepository;
            this._IClientService = _iClientService;
            this._ITransferRepository = _iTransferRepository;
            this._IAccountService = _iAccountService;
            this._UpdateBalancesUseCase = _updateBalancesUseCase;
            this._INotifyBacenUseCase = _iNotifyBacenUseCase;
            this._Logger = _logger;
            this._UpdateBalancesUseCasePresenter = _updateBalancesUseCasePresenter;
            this._NotifyBacenUseCasePresenter = _notifyBacenUseCasePresenter;
        }

        public async Task<bool> Handle(ExecuteTransferInput message, IOutputPort<ExecuteTransferOutput> outputPort)
        {
            try
            {
                var transfer = new Transfer(
                    message.IdSourceAccount,
                    message.IdDestinationAccount,
                    message.Value,
                    message.IdClient
                );

                this._Logger.LogInformation($"Started transfer process and checking if transfer already exists. Transfer: {transfer.IdClient}. DataTime: {DateTime.Now}");
                var idempotence = await this._IIdempotenceRepository.GetByIdempotenceHash(transfer.IdempotenceHash);

                if (idempotence is not null)
                {
                    this._Logger.LogError($"Transfer already executed. Transfer: {transfer.IdClient}. DataTime: {DateTime.Now}");
                    outputPort.Handle(new ExecuteTransferOutput(false, "Transação já efetuada, por favor tentar mais tarde."));
                    return false;
                }

                var client = await this._IClientService.GetClientById(message.IdClient);

                if (client is null)
                {
                    this._Logger.LogError($"Client was not found. Transfer: {transfer.IdClient}. DataTime: {DateTime.Now}");
                    outputPort.Handle(new ExecuteTransferOutput(false, "Dados do cliente não encontrado"));
                    return false;
                }

                var sourceAccount = await this._IAccountService.GetClientById(message.IdSourceAccount);

                if (sourceAccount is null)
                {
                    this._Logger.LogError($"Source account client was not found. Transfer: {transfer.IdClient}. DataTime: {DateTime.Now}");
                    outputPort.Handle(new ExecuteTransferOutput(false, "Dados da conta bancária não encontrados"));
                    return false;
                }

                if (!sourceAccount.AccountIsActive())
                {
                    this._Logger.LogError($"Source account client is inactive. Transfer: {transfer.IdClient}. DataTime: {DateTime.Now}");
                    outputPort.Handle(new ExecuteTransferOutput(false, "Conta bancária inativa"));
                    return false;
                }

                if (sourceAccount.DailyLimitExceeded(transfer.Value))
                {
                    this._Logger.LogError($"Source account client exceeed daily limit transfers. Transfer: {transfer.IdClient}. DataTime: {DateTime.Now}");
                    outputPort.Handle(new ExecuteTransferOutput(false, "Limite diário excedido"));
                    return false;
                }

                if (sourceAccount.InsufficientBalance(transfer.Value))
                {
                    this._Logger.LogError($"Source account client has no suficient balance. Transfer: {transfer.IdClient}. DataTime: {DateTime.Now}");
                    outputPort.Handle(new ExecuteTransferOutput(false, "Saldo bancário insuficente"));
                    return false;
                }

                var idempotenceEntity = new Idempotence(transfer.IdempotenceHash);

                await SaveTransfer(transfer, idempotenceEntity, message.CancellationToken);
                await UpdateBalances(transfer, message.CancellationToken);
                await NotifyBacen(transfer, message.CancellationToken);

                outputPort.Handle(new ExecuteTransferOutput(true, transfer.Id));
                return true;
            }
            catch (ArgumentException validateRulesException)
            {   
                outputPort.Handle(new ExecuteTransferOutput(false, validateRulesException.Message));
                return false;
            }
            catch (Exception exception)
            {
                var idempotenceHash = StringHelper.GenerateIdempotenceHash(message.IdSourceAccount, message.IdDestinationAccount, message.Value);

                var transfer = await this._ITransferRepository.GetByIdempotenceHash(idempotenceHash);
                transfer.StatusId = (int)TransferStatuses.UNKNOWN_FAILURE_ON_PROCESSING_FLOW;

                this._ITransferRepository.Update(transfer);
                await this._ITransferRepository.SaveChangesAsync(message.CancellationToken);

                this._Logger.LogError($"An error has occurred when the request has been processing. Exception: {exception}. DataTime: {DateTime.Now}");
                outputPort.Handle(new ExecuteTransferOutput(false, "An error has occurred when the request has been processing"));
                return false;
            }            
        }

        public async Task<bool> UpdateBalances(Transfer transfer, CancellationToken cancellationToken)
        {
            await _UpdateBalancesUseCase
                .Handle(new UpdateBalancesInput(transfer.IdDestinationAccount, transfer.IdSourceAccount, transfer.Value), this._UpdateBalancesUseCasePresenter)
                .ConfigureAwait(true);

            var updateBalancesOutput = (UpdateBalancesOutput)this._UpdateBalancesUseCasePresenter.GetUseCaseResponse();
            transfer.StatusId = (int)updateBalancesOutput.Status;

            this._ITransferRepository.Update(transfer);
            await this._ITransferRepository.SaveChangesAsync(cancellationToken);
            
            this._Logger.LogInformation($"Performed balance update on external API. Transfer: {transfer.IdClient}. Status: {transfer.StatusId} DataTime: {DateTime.Now}");
            return true;
        }

        public async Task<bool> FindTransferAndUpdateForFailure(Guid idTransfer, CancellationToken cancellationToken)
        {
            var transfer = await this._ITransferRepository.GetByIdAsync(idTransfer, cancellationToken);
            transfer.StatusId = (int)TransferStatuses.UNKNOWN_FAILURE_ON_PROCESSING_FLOW;

            this._ITransferRepository.Update(transfer);
            await this._ITransferRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> NotifyBacen(Transfer transfer, CancellationToken cancellationToken)
        {
            await this._INotifyBacenUseCase
                .Handle(new NotifyBacenUseCaseInput(transfer.Value, transfer.IdSourceAccount, transfer.IdDestinationAccount), this._NotifyBacenUseCasePresenter)
                .ConfigureAwait(true);

            var notifyBacenOutput = (NotifyBacenUseCaseOutput)this._NotifyBacenUseCasePresenter.GetUseCaseResponse();

            if (notifyBacenOutput.Success)
                transfer.StatusId = (int)TransferStatuses.SUCCESSFULLY_TRANSFER;
            else
                transfer.StatusId = (int)TransferStatuses.FAILURE_NOTIFY_BANCEN;

            this._Logger.LogInformation($"Performed bacen notification on external API. Transfer: {transfer.IdClient}. Status: {transfer.StatusId} DataTime: {DateTime.Now}");
            this._ITransferRepository.Update(transfer);
            await this._ITransferRepository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> SaveTransfer(Transfer transfer, Idempotence idempotence, CancellationToken cancellationToken)
        {
            await this._ITransferRepository.AddAsync(transfer, cancellationToken);
            await this._ITransferRepository.SaveChangesAsync(cancellationToken);

            await this._IIdempotenceRepository.AddAsync(idempotence, cancellationToken);
            await this._IIdempotenceRepository.SaveChangesAsync(cancellationToken);

            this._Logger.LogInformation($"Transfer saved in database. Transfer: {transfer.IdClient}. DataTime: {DateTime.Now}");
            return true;
        }
    }
}
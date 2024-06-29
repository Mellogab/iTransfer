using iTransferencia.Core;
using iTransferencia.Core.Entities;
using iTransferencia.Core.Entities.Adapters;
using iTransferencia.Core.Enums;
using iTransferencia.Core.Repository;
using iTransferencia.Core.Services;
using iTransferencia.Core.UseCases.Accounts.UpdateBalances;
using iTransferencia.Core.UseCases.Bacen.NotifyBacen;
using iTransferencia.Core.UseCases.Transfers.ExecuteTransfer;
using iTransferencia.Presenters;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace iTransferencia.UnitTests.UseCases
{
    public class ExecuteTransferUseCaseTests
    {
        [Fact(DisplayName = "Execute transfer given two bank accounts and one clientId")]
        [Trait("Transfer", "Succesfully Operation")]
        public async Task Execute_transfer_given_two_bank_accounts_and_one_clientId()
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<ExecuteTransferUseCase>();

            var presenter = new DefaultPresenter<ExecuteTransferOutput>();

            var client = new ClientService() { 
                id = "bcdd1048-a501-4608-bc82-66d7b4db3600", 
                nome = "João Silva", 
                tipoPessoa = "Fisica", 
                telefone = "912348765" 
            };
            
            var sourceAccountClient = new AccountService() { 
                id = "d0d32142-74b7-4aca-9c68-838aeacef96b", 
                ativo = true, 
                limiteDiario = 500, 
                saldo = 5000 
            };

            _mocker
                .GetMock<IIdempotenceRepository>()
                .Setup(x => x.GetByIdempotenceHash(It.IsAny<string>()))
                .Returns(() => Task.FromResult<Idempotence>(null));

            _mocker
                .GetMock<IClientService>()
                .Setup(x => x.GetClientById(It.IsAny<string>()))
                .Returns(() => Task.FromResult(client));

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.IsAny<string>()))
                .Returns(() => Task.FromResult(sourceAccountClient));

            _mocker
                .GetMock<IIdempotenceRepository>()
                .Setup(x => x.AddAsync(It.IsAny<Idempotence>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult<Idempotence>(null));

            _mocker
                .GetMock<ITransferRepository>()
                .Setup(x => x.AddAsync(It.IsAny<Transfer>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult<Transfer>(null));

            _mocker
                .GetMock<IIdempotenceRepository>()
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(1));

            _mocker
                .GetMock<ITransferRepository>()
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(1));

            var updateBalancesOutputpresenter = new Core.Presenters.DefaultPresenter<UpdateBalancesOutput>();
            updateBalancesOutputpresenter.Handle(new UpdateBalancesOutput(TransferStatuses.SUCCESSFULLY_UPDATE_BALANCES, true));

            _mocker
                .GetMock<IUpdateBalancesUseCase>()
                .Setup(x => x.Handle(It.IsAny<UpdateBalancesInput>(), It.IsAny<IOutputPort<UpdateBalancesOutput>>()))
                .Returns(() => Task.FromResult(true));

            _mocker
                .GetMock<ITransferRepository>()
                .Setup(x => x.Update(It.IsAny<Transfer>()));

            var notifyBacenUseCaseOutputpresenter = new Core.Presenters.DefaultPresenter<NotifyBacenUseCaseOutput>();
            notifyBacenUseCaseOutputpresenter.Handle(new NotifyBacenUseCaseOutput(true));

            _mocker
                .GetMock<INotifyBacenUseCase>()
                .Setup(x => x.Handle(It.IsAny<NotifyBacenUseCaseInput>(), It.IsAny<IOutputPort<NotifyBacenUseCaseOutput>>()))
                .Returns(() => Task.FromResult(true));

            //Act
            var sut = new ExecuteTransferUseCase(
                _mocker.GetMock<IIdempotenceRepository>().Object,
                _mocker.GetMock<ITransferRepository>().Object,
                _mocker.GetMock<IClientService>().Object,
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<IUpdateBalancesUseCase>().Object,
                _mocker.GetMock<INotifyBacenUseCase>().Object,
                _mocker.GetMock<ILogger<ExecuteTransferUseCase>>().Object,
                updateBalancesOutputpresenter,
                notifyBacenUseCaseOutputpresenter
            );

            var executeTransferUseCaseInput = new ExecuteTransferInput {
                CancellationToken = CancellationToken.None,
                IdClient = "2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f",
                Value = 150,
                IdDestinationAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                IdSourceAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
            };

            await sut.Handle(executeTransferUseCaseInput, presenter);

            var result = presenter.GetJsonResult();
            var executeTransferOutput = (ExecuteTransferOutput)result.Value;

            //Assert
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(true, executeTransferOutput.Success);
            Assert.NotNull(executeTransferOutput.transaction_id);
        }

        [Fact(DisplayName = "Get an error when the client was not found on external API")]
        [Trait("Transfer", "Error Operation")]
        public async Task Get_an_error_when_the_clientId_was_not_found_on_external_API()
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<ExecuteTransferUseCase>();

            var presenter = new DefaultPresenter<ExecuteTransferOutput>();

            var client = new ClientService()
            {
                id = "bcdd1048-a501-4608-bc82-66d7b4db3600",
                nome = "João Silva",
                tipoPessoa = "Fisica",
                telefone = "912348765"
            };

            var sourceAccountClient = new AccountService()
            {
                id = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                ativo = true,
                limiteDiario = 500,
                saldo = 5000
            };

            _mocker
                .GetMock<IIdempotenceRepository>()
                .Setup(x => x.GetByIdempotenceHash(It.IsAny<string>()))
                .Returns(() => Task.FromResult<Idempotence>(null));

            _mocker
                .GetMock<IClientService>()
                .Setup(x => x.GetClientById(It.IsAny<string>()))
                .Returns(() => Task.FromResult<ClientService>(null));

            var updateBalancesOutputpresenter = new Core.Presenters.DefaultPresenter<UpdateBalancesOutput>();
            var notifyBacenUseCaseOutputpresenter = new Core.Presenters.DefaultPresenter<NotifyBacenUseCaseOutput>();
            
            //Act
            var sut = new ExecuteTransferUseCase(
                _mocker.GetMock<IIdempotenceRepository>().Object,
                _mocker.GetMock<ITransferRepository>().Object,
                _mocker.GetMock<IClientService>().Object,
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<IUpdateBalancesUseCase>().Object,
                _mocker.GetMock<INotifyBacenUseCase>().Object,
                _mocker.GetMock<ILogger<ExecuteTransferUseCase>>().Object,
                updateBalancesOutputpresenter,
                notifyBacenUseCaseOutputpresenter
            );

            var executeTransferUseCaseInput = new ExecuteTransferInput
            {
                CancellationToken = CancellationToken.None,
                IdClient = "2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f",
                Value = 150,
                IdDestinationAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                IdSourceAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
            };

            await sut.Handle(executeTransferUseCaseInput, presenter);

            var result = presenter.GetJsonResult();
            var executeTransferOutput = (ExecuteTransferOutput)result.Value;

            //Assert
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(false, executeTransferOutput.Success);
            Assert.Equal(executeTransferOutput.Message, "Dados do cliente não encontrado");
        }

        [Fact(DisplayName = "Get an error when the request body was inform that transfer was already performed")]
        [Trait("Transfer", "Error Operation")]
        public async Task Get_an_error_when_the_request_body_was_inform_that_transfer_was_already_performed()
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<ExecuteTransferUseCase>();

            var presenter = new DefaultPresenter<ExecuteTransferOutput>();

            var client = new ClientService()
            {
                id = "bcdd1048-a501-4608-bc82-66d7b4db3600",
                nome = "João Silva",
                tipoPessoa = "Fisica",
                telefone = "912348765"
            };

            var sourceAccountClient = new AccountService()
            {
                id = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                ativo = true,
                limiteDiario = 500,
                saldo = 5000
            };

            _mocker
                .GetMock<IIdempotenceRepository>()
                .Setup(x => x.GetByIdempotenceHash(It.IsAny<string>()))
                .Returns(() => Task.FromResult<Idempotence>(new Idempotence()));

            var updateBalancesOutputpresenter = new Core.Presenters.DefaultPresenter<UpdateBalancesOutput>();
            var notifyBacenUseCaseOutputpresenter = new Core.Presenters.DefaultPresenter<NotifyBacenUseCaseOutput>();

            //Act
            var sut = new ExecuteTransferUseCase(
                _mocker.GetMock<IIdempotenceRepository>().Object,
                _mocker.GetMock<ITransferRepository>().Object,
                _mocker.GetMock<IClientService>().Object,
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<IUpdateBalancesUseCase>().Object,
                _mocker.GetMock<INotifyBacenUseCase>().Object,
                _mocker.GetMock<ILogger<ExecuteTransferUseCase>>().Object,
                updateBalancesOutputpresenter,
                notifyBacenUseCaseOutputpresenter
            );

            var executeTransferUseCaseInput = new ExecuteTransferInput
            {
                CancellationToken = CancellationToken.None,
                IdClient = "2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f",
                Value = 150,
                IdDestinationAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                IdSourceAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
            };

            await sut.Handle(executeTransferUseCaseInput, presenter);

            var result = presenter.GetJsonResult();
            var executeTransferOutput = (ExecuteTransferOutput)result.Value;

            //Assert
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(false, executeTransferOutput.Success);
            Assert.Equal(executeTransferOutput.Message, "Transação já efetuada, por favor tentar mais tarde.");
        }

        [Fact(DisplayName = "Get an error when the bank account was not found on external API")]
        [Trait("Transfer", "Error Operation")]
        public async Task Get_an_error_when_the_bankAccount_was_not_found_on_external_API()
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<ExecuteTransferUseCase>();

            var presenter = new DefaultPresenter<ExecuteTransferOutput>();

            var client = new ClientService()
            {
                id = "bcdd1048-a501-4608-bc82-66d7b4db3600",
                nome = "João Silva",
                tipoPessoa = "Fisica",
                telefone = "912348765"
            };

            var sourceAccountClient = new AccountService()
            {
                id = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                ativo = true,
                limiteDiario = 500,
                saldo = 5000
            };

            _mocker
                .GetMock<IIdempotenceRepository>()
                .Setup(x => x.GetByIdempotenceHash(It.IsAny<string>()))
                .Returns(() => Task.FromResult<Idempotence>(null));

            _mocker
                .GetMock<IClientService>()
                .Setup(x => x.GetClientById(It.IsAny<string>()))
                .Returns(() => Task.FromResult(client));

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.IsAny<string>()))
                .Returns(() => Task.FromResult<AccountService>(null));

            var updateBalancesOutputpresenter = new Core.Presenters.DefaultPresenter<UpdateBalancesOutput>();
            var notifyBacenUseCaseOutputpresenter = new Core.Presenters.DefaultPresenter<NotifyBacenUseCaseOutput>();
            notifyBacenUseCaseOutputpresenter.Handle(new NotifyBacenUseCaseOutput(true));

            //Act
            var sut = new ExecuteTransferUseCase(
                _mocker.GetMock<IIdempotenceRepository>().Object,
                _mocker.GetMock<ITransferRepository>().Object,
                _mocker.GetMock<IClientService>().Object,
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<IUpdateBalancesUseCase>().Object,
                _mocker.GetMock<INotifyBacenUseCase>().Object,
                _mocker.GetMock<ILogger<ExecuteTransferUseCase>>().Object,
                updateBalancesOutputpresenter,
                notifyBacenUseCaseOutputpresenter
            );

            var executeTransferUseCaseInput = new ExecuteTransferInput
            {
                CancellationToken = CancellationToken.None,
                IdClient = "2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f",
                Value = 150,
                IdDestinationAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                IdSourceAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
            };

            await sut.Handle(executeTransferUseCaseInput, presenter);

            var result = presenter.GetJsonResult();
            var executeTransferOutput = (ExecuteTransferOutput)result.Value;

            //Assert
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(false, executeTransferOutput.Success);
            Assert.Equal(executeTransferOutput.Message, "Dados da conta bancária não encontrados");
        }

        [Fact(DisplayName = "Get an error when the bank account was inactive")]
        [Trait("Transfer", "Error Operation")]
        public async Task Get_an_error_when_the_bank_account_was_inactive()
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<ExecuteTransferUseCase>();

            var presenter = new DefaultPresenter<ExecuteTransferOutput>();

            var client = new ClientService()
            {
                id = "bcdd1048-a501-4608-bc82-66d7b4db3600",
                nome = "João Silva",
                tipoPessoa = "Fisica",
                telefone = "912348765"
            };

            var sourceAccountClient = new AccountService()
            {
                id = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                ativo = false,
                limiteDiario = 500,
                saldo = 5000
            };

            _mocker
                .GetMock<IIdempotenceRepository>()
                .Setup(x => x.GetByIdempotenceHash(It.IsAny<string>()))
                .Returns(() => Task.FromResult<Idempotence>(null));

            _mocker
                .GetMock<IClientService>()
                .Setup(x => x.GetClientById(It.IsAny<string>()))
                .Returns(() => Task.FromResult(client));

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.IsAny<string>()))
                .Returns(() => Task.FromResult(sourceAccountClient));

            var updateBalancesOutputpresenter = new Core.Presenters.DefaultPresenter<UpdateBalancesOutput>();
            var notifyBacenUseCaseOutputpresenter = new Core.Presenters.DefaultPresenter<NotifyBacenUseCaseOutput>();
            notifyBacenUseCaseOutputpresenter.Handle(new NotifyBacenUseCaseOutput(true));

            //Act
            var sut = new ExecuteTransferUseCase(
                _mocker.GetMock<IIdempotenceRepository>().Object,
                _mocker.GetMock<ITransferRepository>().Object,
                _mocker.GetMock<IClientService>().Object,
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<IUpdateBalancesUseCase>().Object,
                _mocker.GetMock<INotifyBacenUseCase>().Object,
                _mocker.GetMock<ILogger<ExecuteTransferUseCase>>().Object,
                updateBalancesOutputpresenter,
                notifyBacenUseCaseOutputpresenter
            );

            var executeTransferUseCaseInput = new ExecuteTransferInput
            {
                CancellationToken = CancellationToken.None,
                IdClient = "2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f",
                Value = 150,
                IdDestinationAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                IdSourceAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
            };

            await sut.Handle(executeTransferUseCaseInput, presenter);

            var result = presenter.GetJsonResult();
            var executeTransferOutput = (ExecuteTransferOutput)result.Value;

            //Assert
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(false, executeTransferOutput.Success);
            Assert.Equal(executeTransferOutput.Message, "Conta bancária inativa");
        }

        [Fact(DisplayName = "Get an error when the transfer daily limit was exceeded")]
        [Trait("Transfer", "Error Operation")]
        public async Task Get_an_error_when_the_transfer_daily_limit_was_exceeded()
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<ExecuteTransferUseCase>();

            var presenter = new DefaultPresenter<ExecuteTransferOutput>();

            var client = new ClientService()
            {
                id = "bcdd1048-a501-4608-bc82-66d7b4db3600",
                nome = "João Silva",
                tipoPessoa = "Fisica",
                telefone = "912348765"
            };

            var sourceAccountClient = new AccountService()
            {
                id = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                ativo = true,
                limiteDiario = 500,
                saldo = 5000
            };

            _mocker
                .GetMock<IIdempotenceRepository>()
                .Setup(x => x.GetByIdempotenceHash(It.IsAny<string>()))
                .Returns(() => Task.FromResult<Idempotence>(null));

            _mocker
                .GetMock<IClientService>()
                .Setup(x => x.GetClientById(It.IsAny<string>()))
                .Returns(() => Task.FromResult(client));

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.IsAny<string>()))
                .Returns(() => Task.FromResult(sourceAccountClient));

            var updateBalancesOutputpresenter = new Core.Presenters.DefaultPresenter<UpdateBalancesOutput>();
            var notifyBacenUseCaseOutputpresenter = new Core.Presenters.DefaultPresenter<NotifyBacenUseCaseOutput>();
            notifyBacenUseCaseOutputpresenter.Handle(new NotifyBacenUseCaseOutput(true));

            //Act
            var sut = new ExecuteTransferUseCase(
                _mocker.GetMock<IIdempotenceRepository>().Object,
                _mocker.GetMock<ITransferRepository>().Object,
                _mocker.GetMock<IClientService>().Object,
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<IUpdateBalancesUseCase>().Object,
                _mocker.GetMock<INotifyBacenUseCase>().Object,
                _mocker.GetMock<ILogger<ExecuteTransferUseCase>>().Object,
                updateBalancesOutputpresenter,
                notifyBacenUseCaseOutputpresenter
            );

            var executeTransferUseCaseInput = new ExecuteTransferInput
            {
                CancellationToken = CancellationToken.None,
                IdClient = "2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f",
                Value = 600,
                IdDestinationAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                IdSourceAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
            };

            await sut.Handle(executeTransferUseCaseInput, presenter);

            var result = presenter.GetJsonResult();
            var executeTransferOutput = (ExecuteTransferOutput)result.Value;

            //Assert
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(false, executeTransferOutput.Success);
            Assert.Equal(executeTransferOutput.Message, "Limite diário excedido");
        }

        [Fact(DisplayName = "Get an error when the account has insufficient balance")]
        [Trait("Transfer", "Error Operation")]
        public async Task Get_an_error_when_the_account_has_insufficient_balance()
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<ExecuteTransferUseCase>();

            var presenter = new DefaultPresenter<ExecuteTransferOutput>();

            var client = new ClientService()
            {
                id = "bcdd1048-a501-4608-bc82-66d7b4db3600",
                nome = "João Silva",
                tipoPessoa = "Fisica",
                telefone = "912348765"
            };

            var sourceAccountClient = new AccountService()
            {
                id = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                ativo = true,
                limiteDiario = 500,
                saldo = 100
            };

            _mocker
                .GetMock<IIdempotenceRepository>()
                .Setup(x => x.GetByIdempotenceHash(It.IsAny<string>()))
                .Returns(() => Task.FromResult<Idempotence>(null));

            _mocker
                .GetMock<IClientService>()
                .Setup(x => x.GetClientById(It.IsAny<string>()))
                .Returns(() => Task.FromResult(client));

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.IsAny<string>()))
                .Returns(() => Task.FromResult(sourceAccountClient));

            var updateBalancesOutputpresenter = new Core.Presenters.DefaultPresenter<UpdateBalancesOutput>();
            var notifyBacenUseCaseOutputpresenter = new Core.Presenters.DefaultPresenter<NotifyBacenUseCaseOutput>();
            notifyBacenUseCaseOutputpresenter.Handle(new NotifyBacenUseCaseOutput(true));

            //Act
            var sut = new ExecuteTransferUseCase(
                _mocker.GetMock<IIdempotenceRepository>().Object,
                _mocker.GetMock<ITransferRepository>().Object,
                _mocker.GetMock<IClientService>().Object,
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<IUpdateBalancesUseCase>().Object,
                _mocker.GetMock<INotifyBacenUseCase>().Object,
                _mocker.GetMock<ILogger<ExecuteTransferUseCase>>().Object,
                updateBalancesOutputpresenter,
                notifyBacenUseCaseOutputpresenter
            );

            var executeTransferUseCaseInput = new ExecuteTransferInput
            {
                CancellationToken = CancellationToken.None,
                IdClient = "2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f",
                Value = 350,
                IdDestinationAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                IdSourceAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
            };

            await sut.Handle(executeTransferUseCaseInput, presenter);

            var result = presenter.GetJsonResult();
            var executeTransferOutput = (ExecuteTransferOutput)result.Value;

            //Assert
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(false, executeTransferOutput.Success);
            Assert.Equal(executeTransferOutput.Message, "Saldo bancário insuficente");
        }

        [Theory]
        [Trait("Transfer", "Error Operation")]
        [InlineData("d0d32142-74b7-4aca-9c68-838aeacef96b", "41313d7b-bd75-4c75-9dea-1f4be434007f", 100.00, null, "É necessário informar o código do cliente")]
        [InlineData(null, "41313d7b-bd75-4c75-9dea-1f4be434007f", 100.00, "2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f", "É necessário informar a conta origem")]
        [InlineData("d0d32142-74b7-4aca-9c68-838aeacef96b",null, 100.00, "2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f", "É nccessário informar a conta destino")]
        [InlineData("d0d32142-74b7-4aca-9c68-838aeacef96b", "41313d7b-bd75-4c75-9dea-1f4be434007f", 00, "2ceb26e9-7b5c-417e-bf75-ffaa66e3a76f", "É nccessário informar o valor")]
        public async Task Get_an_error_when_the_input_parameters_wasnt_informed_correctly(string sourceAccountId, string destinationAccountId, decimal value, string clientId, string messageError)
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<ExecuteTransferUseCase>();

            var presenter = new DefaultPresenter<ExecuteTransferOutput>();

            var updateBalancesOutputpresenter = new Core.Presenters.DefaultPresenter<UpdateBalancesOutput>();
            var notifyBacenUseCaseOutputpresenter = new Core.Presenters.DefaultPresenter<NotifyBacenUseCaseOutput>();
           
            //Act
            var sut = new ExecuteTransferUseCase(
                _mocker.GetMock<IIdempotenceRepository>().Object,
                _mocker.GetMock<ITransferRepository>().Object,
                _mocker.GetMock<IClientService>().Object,
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<IUpdateBalancesUseCase>().Object,
                _mocker.GetMock<INotifyBacenUseCase>().Object,
                _mocker.GetMock<ILogger<ExecuteTransferUseCase>>().Object,
                updateBalancesOutputpresenter,
                notifyBacenUseCaseOutputpresenter
            );

            var executeTransferUseCaseInput = new ExecuteTransferInput
            {
                CancellationToken = CancellationToken.None,
                IdClient = clientId,
                Value = value,
                IdDestinationAccount = destinationAccountId,
                IdSourceAccount = sourceAccountId,
            };

            await sut.Handle(executeTransferUseCaseInput, presenter);

            var result = presenter.GetJsonResult();
            var executeTransferOutput = (ExecuteTransferOutput)result.Value;

            //Assert
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(false, executeTransferOutput.Success);
            Assert.Equal(executeTransferOutput.Message, messageError);
        }
    }
}
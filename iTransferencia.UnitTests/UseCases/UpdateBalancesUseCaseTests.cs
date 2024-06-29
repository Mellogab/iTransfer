using iTransferencia.Core.Enums;
using iTransferencia.Core.Patterns;
using iTransferencia.Core.Services;
using iTransferencia.Core.UseCases.Accounts.UpdateBalances;
using iTransferencia.Core.UseCases.Transfers.ExecuteTransfer;
using iTransferencia.Infrastructure.Patterns.Sagas;
using iTransferencia.Presenters;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace iTransferencia.UnitTests.UseCases
{
    public class UpdateBalancesUseCaseTests
    {
        [Fact(DisplayName = "Update Balances Succesfully")]
        [Trait("UpdateBalances", "Succesfully Operation")]
        public async Task Update_ballances_succesfully()
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<ExecuteTransferUseCase>();

            var presenter = new DefaultPresenter<UpdateBalancesOutput>();
            var sourceAccount = new Core.Entities.Adapters.AccountService("", 1000, true, 1000);
            var destionationAccount = new Core.Entities.Adapters.AccountService("", 1000, true, 1000);

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.Is<string>(arg => arg == "d0d32142-74b7-4aca-9c68-838aeacef96b")))
                .Returns(() => Task.FromResult(sourceAccount));

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.Is<string>(arg => arg == "41313d7b-bd75-4c75-9dea-1f4be434007f")))
                .Returns(() => Task.FromResult(destionationAccount));

            _mocker
                .GetMock<ISagaOrquestrator>()
                .Setup(x => x.ExecuteAsync());

            //Act
            var sut = new UpdateBalancesUseCase(
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<ISagaOrquestrator>().Object
            );

            var UpdateBalancesInput = new UpdateBalancesInput
            {
                IdDestinationAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                IdSourceAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                SourceBalance = 1000,
                Value = 100
            };

            await sut.Handle(UpdateBalancesInput, presenter);
            var result = presenter.GetJsonResult();
            var updateBalancesOutput = (UpdateBalancesOutput)result.Value;

            //Assert
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(true, updateBalancesOutput.Success);
            Assert.Equal(TransferStatuses.SUCCESSFULLY_UPDATE_BALANCES, updateBalancesOutput.Status);
        }

        [Fact(DisplayName = "Give an error when source account was not found")]
        [Trait("UpdateBalances", "Error Operation")]
        public async Task Give_an_error_when_source_account_was_not_found()
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<ExecuteTransferUseCase>();

            var presenter = new DefaultPresenter<UpdateBalancesOutput>();
            var sourceAccount = new Core.Entities.Adapters.AccountService("", 1000, true, 1000);
            var destionationAccount = new Core.Entities.Adapters.AccountService("", 1000, true, 1000);

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.Is<string>(arg => arg == "d0d32142-74b7-4aca-9c68-838aeacef96b")))
                .Returns(() => Task.FromResult<Core.Entities.Adapters.AccountService>((null)));

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.Is<string>(arg => arg == "41313d7b-bd75-4c75-9dea-1f4be434007f")))
                .Returns(() => Task.FromResult<Core.Entities.Adapters.AccountService>(null));

            //Act
            var sut = new UpdateBalancesUseCase(
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<ISagaOrquestrator>().Object
            );

            var UpdateBalancesInput = new UpdateBalancesInput
            {
                IdDestinationAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                IdSourceAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                SourceBalance = 1000,
                Value = 100
            };

            await sut.Handle(UpdateBalancesInput, presenter);
            var result = presenter.GetJsonResult();
            var updateBalancesOutput = (UpdateBalancesOutput)result.Value;

            //Assert
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(false, updateBalancesOutput.Success);
            Assert.Equal(updateBalancesOutput.Message, "Conta origem não encontrada");
        }

        [Fact(DisplayName = "Give an error when destination account was not found")]
        [Trait("UpdateBalances", "Error Operation")]
        public async Task Give_an_error_when_destination_account_was_not_found()
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<ExecuteTransferUseCase>();

            var presenter = new DefaultPresenter<UpdateBalancesOutput>();
            var sourceAccount = new Core.Entities.Adapters.AccountService("", 1000, true, 1000);
            var destionationAccount = new Core.Entities.Adapters.AccountService("", 1000, true, 1000);

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.Is<string>(arg => arg == "d0d32142-74b7-4aca-9c68-838aeacef96b")))
                .Returns(() => Task.FromResult(sourceAccount));

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.Is<string>(arg => arg == "41313d7b-bd75-4c75-9dea-1f4be434007f")))
                .Returns(() => Task.FromResult<Core.Entities.Adapters.AccountService>(null));

            //Act
            var sut = new UpdateBalancesUseCase(
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<ISagaOrquestrator>().Object
            );

            var UpdateBalancesInput = new UpdateBalancesInput
            {
                IdDestinationAccount = "41313d7b-bd75-4c75-9dea-1f4be434007f",
                IdSourceAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                SourceBalance = 1000,
                Value = 100
            };

            await sut.Handle(UpdateBalancesInput, presenter);
            var result = presenter.GetJsonResult();
            var updateBalancesOutput = (UpdateBalancesOutput)result.Value;

            //Assert
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(false, updateBalancesOutput.Success);
            Assert.Equal(updateBalancesOutput.Message, "Conta destino não encontrada");
        }

        [Fact(DisplayName = "Execute compensation process when something had been wrong")]
        [Trait("UpdateBalances", "Error Operation")]
        public async Task Execute_compensation_process_when_something_had_been_wrong()
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<ExecuteTransferUseCase>();

            var presenter = new DefaultPresenter<UpdateBalancesOutput>();
            var sourceAccount = new Core.Entities.Adapters.AccountService("", 1000, true, 1000);
            var destionationAccount = new Core.Entities.Adapters.AccountService("", 1000, true, 1000);

            var sagaOrquestrator = _mocker.CreateInstance<SagaOrquestrator>();

            sagaOrquestrator.AddStep(
                 async () => await Execution(),
                 async () => await Compensation()
             );

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.Is<string>(arg => arg == "d0d32142-74b7-4aca-9c68-838aeacef96b")))
                .Returns(() => Task.FromResult(sourceAccount));

            _mocker
                .GetMock<IAccountService>()
                .Setup(x => x.GetClientById(It.Is<string>(arg => arg == "41313d7b-bd75-4c75-9dea-1f4be434007f")))
                .Returns(() => Task.FromResult(destionationAccount));

            _mocker
                .GetMock<ISagaOrquestrator>()
                .Setup(x => x.ExecuteAsync());

            //Act
            var sut = new UpdateBalancesUseCase(
                _mocker.GetMock<IAccountService>().Object,
                _mocker.GetMock<IAccountService>().Object,
                sagaOrquestrator
            );

            var UpdateBalancesInput = new UpdateBalancesInput
            {
                IdDestinationAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                IdSourceAccount = "d0d32142-74b7-4aca-9c68-838aeacef96b",
                SourceBalance = 1000,
                Value = 100
            };

            await sut.Handle(UpdateBalancesInput, presenter);
            var result = presenter.GetJsonResult();
            var updateBalancesOutput = (UpdateBalancesOutput)result.Value;

            //Assert
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(false, updateBalancesOutput.Success);
            Assert.Equal(TransferStatuses.FAILURE_UPDATE_BALANCES, updateBalancesOutput.Status);
        }

        private async Task Execution()
        {
            throw new InvalidOperationException("exception");
        }

        private async Task Compensation() { }

    }   
}

using iTransferencia.Infrastructure.Patterns.Sagas;
using Moq.AutoMock;
using Xunit;

namespace iTransferencia.UnitTests.Patterns
{
    public class SagaOrquestratorTests
    {
        [Fact(DisplayName = "Execute all steps successfully")]
        [Trait("SagaOrquestrator", "Success Operation")]
        public async Task ExecuteAllStepsSuccessfully()
        {
            // Arrange
            var _mocker = new AutoMocker();
            var sagaOrquestrator = _mocker.CreateInstance<SagaOrquestrator>();

            bool operation1Executed = false;
            bool operation2Executed = false;

            sagaOrquestrator.AddStep(
                () =>
                {
                    operation1Executed = true;
                    return Task.CompletedTask;
                },
                () => Task.CompletedTask);

            sagaOrquestrator.AddStep(
                () =>
                {
                    operation2Executed = true;
                    return Task.CompletedTask;
                },
                () => Task.CompletedTask);

            // Act
            await sagaOrquestrator.ExecuteAsync();

            // Assert
            Assert.True(operation1Executed);
            Assert.True(operation2Executed);
        }

        [Fact(DisplayName = "Execute compensation steps when an operation fails")]
        [Trait("SagaOrquestrator", "Compensation Operation")]
        public async Task ExecuteCompensationStepsWhenAnOperationFails()
        {
            // Arrange
            var _mocker = new AutoMocker();
            var sagaOrquestrator = _mocker.CreateInstance<SagaOrquestrator>();

            bool operation1Executed = false;
            bool operation2Executed = false;
            bool compensation1Executed = false;

            sagaOrquestrator.AddStep(
                () =>
                {
                    operation1Executed = true;
                    return Task.CompletedTask;
                },
                () =>
                {
                    compensation1Executed = true;
                    return Task.CompletedTask;
                });

            sagaOrquestrator.AddStep(
                () =>
                {
                    operation2Executed = false;
                    throw new Exception("Operation 2 failed");
                },
                () => Task.CompletedTask);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await sagaOrquestrator.ExecuteAsync());

            Assert.Equal("Operation 2 failed", exception.Message);
            Assert.True(operation1Executed);
            Assert.True(compensation1Executed);
            Assert.False(operation2Executed);
        }

        [Fact(DisplayName = "Compensate multiple steps in reverse order")]
        [Trait("SagaOrquestrator", "Compensation Order")]
        public async Task CompensateMultipleStepsInReverseOrder()
        {
            // Arrange
            var _mocker = new AutoMocker();
            var sagaOrquestrator = _mocker.CreateInstance<SagaOrquestrator>();

            bool operation1Executed = false;
            bool operation2Executed = false;
            bool compensation1Executed = false;
            bool compensation2Executed = false;

            sagaOrquestrator.AddStep(
                () =>
                {
                    operation1Executed = true;
                    return Task.CompletedTask;
                },
                () =>
                {
                    compensation1Executed = true;
                    return Task.CompletedTask;
                });

            sagaOrquestrator.AddStep(
                () =>
                {
                    operation2Executed = false;
                    throw new Exception("Operation 2 failed");
                },
                () =>
                {
                    compensation2Executed = true;
                    return Task.CompletedTask;
                });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(async () => await sagaOrquestrator.ExecuteAsync());

            Assert.Equal("Operation 2 failed", exception.Message);
            Assert.True(operation1Executed);
            Assert.False(operation2Executed);
            Assert.True(compensation2Executed);
            Assert.True(compensation1Executed);
        }
    }
}

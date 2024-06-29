using AutoMapper;
using iTransferencia.Core.Entities.Adapters;
using iTransferencia.Core.Services;
using iTransferencia.Core.UseCases.Bacen.NotifyBacen;
using iTransferencia.Core;
using Moq;
using Xunit;

namespace iTransferencia.UnitTests.UseCases
{
    public class NotifyBacenUseCaseTests
    {
        private readonly Mock<IHttpRequestService> _mockHttpRequestService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BacenAPI _bacenAPI;
        private readonly NotifyBacenUseCase _useCase;

        public NotifyBacenUseCaseTests()
        {
            _mockHttpRequestService = new Mock<IHttpRequestService>();
            _mockMapper = new Mock<IMapper>();
            _bacenAPI = new BacenAPI
            {
                baseURL = "https://api.bacen.gov.br",
                notify = "notify"
            };
            _useCase = new NotifyBacenUseCase(_mockHttpRequestService.Object, _bacenAPI, _mockMapper.Object);
        }

        [Fact(DisplayName = "Handle method successfully notifies Bacen")]
        [Trait("NotifyBacenUseCase", "Handle Operation")]
        public async Task Handle_SuccessfullyNotifiesBacen()
        {
            // Arrange
            var input = new NotifyBacenUseCaseInput { /* Initialize properties */ };
            var moveTransfer = new MoveTransfer { /* Initialize properties */ };
            var outputPort = new Mock<IOutputPort<NotifyBacenUseCaseOutput>>();

            _mockMapper.Setup(m => m.Map<MoveTransfer>(It.IsAny<NotifyBacenUseCaseInput>())).Returns(moveTransfer);
            _mockHttpRequestService.Setup(s => s.MakeRequest<bool>(It.IsAny<HttpRequestUseCaseInput>())).ReturnsAsync(new HttpRequestUseCaseOutput
            {
                Successfully = true,
                Error = null
            });

            // Act
            var result = await _useCase.Handle(input, outputPort.Object);

            // Assert
            Assert.True(result);
            _mockHttpRequestService.Verify(s => s.MakeRequest<bool>(It.Is<HttpRequestUseCaseInput>(r => r.Uri == $"{_bacenAPI.baseURL}/{_bacenAPI.notify}" && r.HttpMethod == HttpMethod.Post && r.Content == moveTransfer)), Times.Once);
            outputPort.Verify(o => o.Handle(It.Is<NotifyBacenUseCaseOutput>(r => r.Success == true && r.Error == null)), Times.Once);
        }
    }
}

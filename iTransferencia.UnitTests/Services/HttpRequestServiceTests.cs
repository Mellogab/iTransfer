using Bogus;
using iTransferencia.Core;
using iTransferencia.Core.Entities.Adapters;
using iTransferencia.Core.Services;
using iTransferencia.Infrastructure.Configurations;
using iTransferencia.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using Moq.Protected;
using System;
using System.Net;
using Xunit;

namespace iTransferencia.UnitTests.Services
{
    public class HttpRequestServiceTests
    {

        [Fact(DisplayName = "Get client by id using fake httpClient")]
        [Trait("Client", "Succesfully Operation")]
        public async Task Get_client_by_id_fake_httpsClient()
        {
            //Arrange
            var _mocker = new AutoMocker();
            _mocker.CreateInstance<HttpRequestService>();

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(" {\r\n      \"id\": \"bcdd1048-a501-4608-bc82-66d7b4db3600\",\r\n      \"nome\": \"João Silva\",\r\n      \"telefone\": \"912348765\",\r\n      \"tipoPessoa\": \"Fisica\"\r\n  }")
            };

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(expectedResponse);

            var httpClient = new HttpClient(handlerMock.Object);

            _mocker
                .GetMock<ICacheService>()
                .Setup(x => x.Get <HttpRequestUseCaseOutput>(It.IsAny<string>()))
                .Returns(() => new HttpRequestUseCaseOutput());

            var retrySettings = new Retry() { ExponentialBackoffBaseSeconds = 1, MaxRetryAttempts = 2 };
            _mocker.Use(retrySettings);

            var circuitBreakerSettings = new CircuitBreaker() { DurationOfBreakMinutes = 2, HandledEventsAllowedBeforeBreaking = 3 };
            _mocker.Use(circuitBreakerSettings);

            _mocker
                .GetMock<ILogger<HttpRequestService>>();

            //Act
            var sut = new HttpRequestService(
                _mocker.GetMock<ICacheService>().Object,
                retrySettings,
                circuitBreakerSettings,
                _mocker.GetMock<ILogger<HttpRequestService>>().Object,
                httpClient
            );

            var result = await sut.MakeRequest<HttpRequestUseCaseOutput>(
                new HttpRequestUseCaseInput()
                {
                    Uri = "https://localhost:1010",
                    Key = "aaa",
                    Content = new byte[] { },
                    Headers = new Dictionary<string, string>(),
                    HttpMethod = HttpMethod.Get,
                }
            );

            //Assert
            Assert.Equal(result.Successfully, true);
            Assert.Contains("bcdd1048-a501-4608-bc82-66d7b4db3600", result.output);
        }
    }
}
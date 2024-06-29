using iTransferencia.Core;
using iTransferencia.Core.Services;
using iTransferencia.Infrastructure.Configurations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Wrap;
using System.Net.Http;
using System.Text;

namespace iTransferencia.Infrastructure.Services
{
    public class HttpRequestService : IHttpRequestService
    {
        private readonly AsyncPolicyWrap<HttpRequestUseCaseOutput> _PolicyWrap;
        private readonly AsyncCircuitBreakerPolicy<HttpRequestUseCaseOutput> _CircuitBreakerPolicy;
        private readonly AsyncRetryPolicy<HttpRequestUseCaseOutput> _RetryPolicy;
        private readonly ICacheService _CacheService;
        private readonly Retry _RetrySettings;
        private readonly CircuitBreaker _CircuitBreakerSettings;
        private readonly ILogger<HttpRequestService> _Logger;
        private readonly HttpClient _HttpClient;

        public int MaxRetryAttempts = 1; 
        public int ExponentialBackoffBaseSeconds = 1;
        public int HandledEventsAllowedBeforeBreaking = 1;
        public int DurationOfBreakMinutes = 1; 

        public HttpRequestService() { }

        public HttpRequestService(
            ICacheService _cacheService,
            Retry retrySettings,
            CircuitBreaker circuitBreakerSettings,
            ILogger<HttpRequestService> _logger,
            HttpClient httpClient
        )
        {
            this._CacheService = _cacheService;
            this._RetrySettings = retrySettings;
            this._CircuitBreakerSettings = circuitBreakerSettings;
            this._Logger = _logger;
            this._HttpClient = httpClient;

            MaxRetryAttempts = this._RetrySettings.MaxRetryAttempts == 0 ? 
                MaxRetryAttempts : this._RetrySettings.MaxRetryAttempts;

            ExponentialBackoffBaseSeconds = this._RetrySettings.ExponentialBackoffBaseSeconds == 0 ? 
                ExponentialBackoffBaseSeconds : this._RetrySettings.ExponentialBackoffBaseSeconds;

            HandledEventsAllowedBeforeBreaking = this._CircuitBreakerSettings.HandledEventsAllowedBeforeBreaking == 0 ? 
                HandledEventsAllowedBeforeBreaking : this._CircuitBreakerSettings.HandledEventsAllowedBeforeBreaking;

            DurationOfBreakMinutes = this._CircuitBreakerSettings.DurationOfBreakMinutes == 0 ? 
                DurationOfBreakMinutes : this._CircuitBreakerSettings.DurationOfBreakMinutes;

            _RetryPolicy = Policy<HttpRequestUseCaseOutput>
                .Handle<HttpRequestException>()
                .OrResult(r => !r.Successfully)
                .WaitAndRetryAsync(MaxRetryAttempts, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(ExponentialBackoffBaseSeconds, retryAttempt))
                );
            
            _CircuitBreakerPolicy = Policy<HttpRequestUseCaseOutput>
                .Handle<HttpRequestException>()
                .OrResult(r => !r.Successfully)
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: HandledEventsAllowedBeforeBreaking,
                    durationOfBreak: TimeSpan.FromMinutes(DurationOfBreakMinutes)
                );
            
            _PolicyWrap = Policy.WrapAsync(_RetryPolicy, _CircuitBreakerPolicy);
        }

        public async Task<HttpRequestUseCaseOutput> MakeRequest<TEntity>(HttpRequestUseCaseInput input)
        {
            if (input.HttpMethod == HttpMethod.Get)
            {
               return await _PolicyWrap
                    .ExecuteAsync(async () => await ExecuteHttpRequest(input))
                    .ContinueWith(async task =>
                    {
                        if (task.Exception != null && task.Exception.InnerException is BrokenCircuitException)
                        {
                            this._Logger.LogError(task.Exception, $"The circuit breaker was triggered due multiple failures to request service: {input.Uri}");
                            var cachedResult = _CacheService.Get<HttpRequestUseCaseOutput>(input.Key);

                            if (cachedResult != null)
                                return cachedResult;
                            
                            throw task.Exception.InnerException;
                        }
                        return await task;
                    }).Unwrap();
            }

            return await ExecuteHttpRequest(input);
        }

        private async Task<HttpRequestUseCaseOutput> ExecuteHttpRequest(HttpRequestUseCaseInput input)
        {
            using (HttpRequestMessage request = new HttpRequestMessage(input.HttpMethod, input.Uri))
            {
                foreach (var (key, value) in input?.Headers)
                    request.Headers.Add(key, value);

                if ((HttpMethod.Post == input.HttpMethod || HttpMethod.Put == input.HttpMethod) && input.Content != null)
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(input.Content), Encoding.UTF8, "application/json");
                    request.Content = stringContent;
                }

                HttpResponseMessage response = await _HttpClient.SendAsync(request);
                HttpRequestUseCaseOutput httpRequestOutput = new HttpRequestUseCaseOutput();

                if (!response.IsSuccessStatusCode)
                {
                    httpRequestOutput.Successfully = false;
                    httpRequestOutput.Error = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
                    return httpRequestOutput;
                }

                HttpContent content = response.Content;
                var contentResponse = await content.ReadAsStringAsync();
                httpRequestOutput.Successfully = true;
                httpRequestOutput.output = contentResponse;

                if (!string.IsNullOrEmpty(input.Key))
                    _CacheService.Set(input.Key, httpRequestOutput, TimeSpan.FromHours(3));

                return httpRequestOutput;
            }
        }
    }
}

using iTransferencia.Core;
using iTransferencia.Core.Entities;
using iTransferencia.Core.Entities.Adapters;
using iTransferencia.Core.Services;

namespace iTransferencia.Infrastructure.Services
{
    public class BacenService : IBacenService
    {
        private IHttpRequestService _HttpRequestService { get; set; }
        private BacenAPI _BacenAPI { get; set; }

        public BacenService(
            IHttpRequestService _httpRequestService,
            BacenAPI _clientAPI
        ) 
        {
            this._HttpRequestService = _httpRequestService;
            this._BacenAPI = _clientAPI;
        } 

        public async Task<HttpRequestUseCaseOutput> NotifyBacen(Transfer Transfer)
        {
            var result = await _HttpRequestService.MakeRequest<bool>(new HttpRequestUseCaseInput(){
                Uri = this._BacenAPI.baseURL + this._BacenAPI.notify,
                HttpMethod = HttpMethod.Post,
                Content = Transfer
            });

            return result;
        }
    }
}

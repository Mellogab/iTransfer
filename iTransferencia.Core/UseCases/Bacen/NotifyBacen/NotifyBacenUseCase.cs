
using AutoMapper;
using iTransferencia.Core.Entities.Adapters;
using iTransferencia.Core.Services;

namespace iTransferencia.Core.UseCases.Bacen.NotifyBacen
{
    public class NotifyBacenUseCase : INotifyBacenUseCase
    {
        private IHttpRequestService _HttpRequestService { get; set; }
        private BacenAPI BacenAPI { get; set; }
        private IMapper _IMapper;


        public NotifyBacenUseCase(
            IHttpRequestService _httpRequestService,
            BacenAPI _clientAPI,
            IMapper _iMapper
        )
        {
            this._HttpRequestService = _httpRequestService;
            this.BacenAPI = _clientAPI;
            this._IMapper = _iMapper;
        }

        public async Task<bool> Handle(NotifyBacenUseCaseInput message, IOutputPort<NotifyBacenUseCaseOutput> outputPort)
        {
            var notifyBacenInput = this._IMapper.Map<MoveTransfer>(message);

            var result = await _HttpRequestService.MakeRequest<bool>(new HttpRequestUseCaseInput()
            {
                Uri = $"{this.BacenAPI.baseURL}/{this.BacenAPI.notify}",
                HttpMethod = HttpMethod.Post,
                Content = notifyBacenInput,
                Headers = new Dictionary<string, string>()
            }); 

            outputPort.Handle(new NotifyBacenUseCaseOutput(result.Successfully, result.Error));
            return true;
        }
    }
}

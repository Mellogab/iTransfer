using iTransferencia.Core.Entities.Adapters;
using iTransferencia.Core.Services;
using iTransferencia.Core;
using Newtonsoft.Json;

namespace iTransferencia.Infrastructure.Services
{
    public class ClientService : IClientService
    {
        private IHttpRequestService _HttpRequestService { get; set; }
        private ClientAPI _ClientAPI { get; set; }

        public ClientService(
            IHttpRequestService _httpRequestService,
            ClientAPI _clientAPI
        )
        {
            this._HttpRequestService = _httpRequestService;
            this._ClientAPI = _clientAPI;
        }

        public async Task<Core.Entities.Adapters.ClientService> GetClientById(string idClient)
        {
            Core.Entities.Adapters.ClientService client = null;

            var result = await _HttpRequestService.MakeRequest<ClientService>(new HttpRequestUseCaseInput()
            {
                Uri = $"{this._ClientAPI.baseURL}/{this._ClientAPI.getClientById}/{idClient}",
                HttpMethod = HttpMethod.Get,
                Key = $"GetClientById-{idClient}"
            });

            if (result.Successfully)
                client = JsonConvert.DeserializeObject<Core.Entities.Adapters.ClientService>(result?.output);

            return client;
        }
    }
}

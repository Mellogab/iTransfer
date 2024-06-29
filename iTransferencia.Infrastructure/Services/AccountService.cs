using iTransferencia.Core.Entities.Adapters;
using iTransferencia.Core.Services;
using iTransferencia.Core;
using Newtonsoft.Json;

namespace iTransferencia.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private IHttpRequestService _HttpRequestService { get; set; }
        private AccountAPI _AccountAPI { get; set; }

        public AccountService(
            IHttpRequestService _httpRequestService,
            AccountAPI _accountAPI
        )
        {
            this._HttpRequestService = _httpRequestService;
            this._AccountAPI = _accountAPI;
        }

        public async Task<Core.Entities.Adapters.AccountService> GetClientById(string idAccount)
        {
            Core.Entities.Adapters.AccountService account = null;

            var result = await _HttpRequestService.MakeRequest<Core.Entities.Adapters.AccountService>(new HttpRequestUseCaseInput()
            {
                Uri = $"{this._AccountAPI.baseURL}/{this._AccountAPI.getAccountById}{idAccount}",
                HttpMethod = HttpMethod.Get,
                Key = $"GetAccountByClientId-{idAccount}"
            });

            if (result.Successfully)
                account = JsonConvert.DeserializeObject<Core.Entities.Adapters.AccountService>(result?.output);
            
            return account;
        }

        public async Task<bool> UpdateBalances(UpdateBalances updateBalance)
        {
            var result = await _HttpRequestService.MakeRequest<UpdateBalances>(new HttpRequestUseCaseInput()
            {
                Uri = $"{this._AccountAPI.baseURL}/{this._AccountAPI.updateBalance}",
                HttpMethod = HttpMethod.Put,
                Content = updateBalance
            });

            return result.Successfully;
        }
    }
}
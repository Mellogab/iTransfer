using iTransferencia.Core.Entities.Adapters;

namespace iTransferencia.Core.Services
{
    public interface IAccountService
    {
        public Task<AccountService> GetClientById(string idAccount);
        public Task<bool> UpdateBalances(UpdateBalances updateBalance);
    }
}

using iTransferencia.Core.Entities.Adapters;

namespace iTransferencia.Core.Services
{
    public interface IClientService
    {
        public Task<ClientService> GetClientById(string idClient);
    }
}

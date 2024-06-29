using iTransferencia.Core.Entities;

namespace iTransferencia.Core.Repository
{
    public interface ITransferRepository : IRepositoryBase<Transfer>
    {
        Task<Transfer> GetByIdempotenceHash(string idempotenceHash);    
    }
}

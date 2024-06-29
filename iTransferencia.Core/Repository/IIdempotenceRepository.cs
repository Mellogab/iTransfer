using iTransferencia.Core.Entities;

namespace iTransferencia.Core.Repository
{
    public interface IIdempotenceRepository : IRepositoryBase<Idempotence>
    {
        Task<Idempotence> GetByIdempotenceHash(string idempotenceHash);
    }
}
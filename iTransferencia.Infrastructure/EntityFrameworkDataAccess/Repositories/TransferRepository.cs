using iTransferencia.Core.Entities;
using iTransferencia.Core.Repository;
using iTransferencia.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace iTransferencia.Infrastructure.EntityFrameworkDataAccess.Repositories
{
    public class TransferRepository(ApplicationDbContext context) : RepositoryBase<Transfer>(context), ITransferRepository
    {
        public Task<Transfer> GetByIdempotenceHash(string idempotenceHash)
        {
            var idempotence = DbSet
                .Where(args => args.IdempotenceHash == idempotenceHash)
                .FirstOrDefaultAsync();

            return idempotence;
        }
    }
}
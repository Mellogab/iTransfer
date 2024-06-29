using iTransferencia.Core.Entities;
using iTransferencia.Core.Repository;
using iTransferencia.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace iTransferencia.Infrastructure.EntityFrameworkDataAccess.Repositories
{
    public class IdempotenceRepository(ApplicationDbContext context) : RepositoryBase<Idempotence>(context), IIdempotenceRepository
    {
        public Task<Idempotence> GetByIdempotenceHash(string idempotenceHash)
        {
            var idempotence = DbSet
                .Where(args => args.IdempotenceHash == idempotenceHash)
                .FirstOrDefaultAsync();

            return idempotence;
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace iTransferencia.Infrastructure.DbContext
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
    {
        public DbSet<Core.Entities.Transfer> Tranfers { get; set; }
        public DbSet<Core.Entities.Idempotence> Idempotence { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            PaymentModelBuilder(modelBuilder);
            IdempotenceModelBuilder(modelBuilder);
        }

        private static void PaymentModelBuilder(ModelBuilder modelBuilder)
        {
            var transfer = modelBuilder.Entity<Core.Entities.Transfer>();

            transfer.ToTable("Transfers");

            transfer.Property(x => x.Id)
                .IsRequired();

            transfer.Property(x => x.IdDestinationAccount)
                .IsRequired();

            transfer.Property(x => x.IdSourceAccount)
                .IsRequired();

            transfer.Property(x => x.StatusId)
                .IsRequired();

            transfer.Property(x => x.OperationDate)
                .IsRequired();

            transfer.Property(x => x.Value)
                .IsRequired();

            transfer.Property(x => x.ExternalTransactionId)
                .IsRequired();

            transfer.Ignore(x => x.IdempotenceHash).HasNoKey();
        }

        private static void IdempotenceModelBuilder(ModelBuilder modelBuilder)
        {
            var idempotenceModelBuilder = modelBuilder.Entity<Core.Entities.Idempotence>();

            idempotenceModelBuilder.ToTable("Idempotence");

            idempotenceModelBuilder.Property(x => x.Id)
                .IsRequired();

            idempotenceModelBuilder.Property(x => x.OperationDate)
                .IsRequired();

            idempotenceModelBuilder.Property(x => x.IdempotenceHash)
                .IsRequired();
        }
    }
}
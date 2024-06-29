using iTransferencia.Core.Entities;
using iTransferencia.Core.Enums;
using Xunit;

namespace iTransferencia.UnitTests.Entities
{
    public class TransferEntityTests
    {
        [Fact]
        public void Constructor_ValidParameters_Success()
        {
            // Arrange
            string idSourceAccount = "sourceAccountId";
            string idDestinationAccount = "destinationAccountId";
            decimal value = 100.00m;
            string idClient = "clientId";

            // Act
            Transfer transfer = new Transfer(idSourceAccount, idDestinationAccount, value, idClient);

            // Assert
            Assert.Equal(idClient, transfer.IdClient);
            Assert.Equal(idSourceAccount, transfer.IdSourceAccount);
            Assert.Equal(idDestinationAccount, transfer.IdDestinationAccount);
            Assert.Equal(value, transfer.Value);
            Assert.Equal((int)TransferStatuses.PROCESSING, transfer.StatusId);
            Assert.NotNull(transfer.IdempotenceHash);
        }

        [Theory]
        [InlineData("", "destinationAccountId", 100.00, "clientId")]
        [InlineData("sourceAccountId", "", 100.00, "clientId")]
        [InlineData("sourceAccountId", "destinationAccountId", 0, "clientId")]
        [InlineData("sourceAccountId", "destinationAccountId", 100.00, "")]
        public void Constructor_InvalidParameters_ArgumentExceptionThrown(string idSourceAccount, string idDestinationAccount, decimal value, string idClient)
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentException>(() => new Transfer(idSourceAccount, idDestinationAccount, value, idClient));
        }
    }
}

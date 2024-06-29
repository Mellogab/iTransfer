using iTransferencia.Core.Enums;

namespace iTransferencia.Core.UseCases.Accounts.UpdateBalances
{
    public class UpdateBalancesOutput : UseCaseResponseMessage
    {
        public TransferStatuses Status;

        public UpdateBalancesOutput(
            bool success,
            string message = null,
            string error = null
        ) : base(success, message, error)
        {
        }

        public UpdateBalancesOutput(
            TransferStatuses status,
            bool success
        ) : base(success, null, null)
        {
            this.Status = status;
        }
    }
}